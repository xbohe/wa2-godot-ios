using Godot;
using System;
using System.Text;
using System.Collections.Generic;
public class FileEntry
{
	public uint Crypted { get; set; }
	public string FileName { get; set; }
	public uint Offset { get; set; }
	public uint Size { get; set; }
	public string PkgPath { get; set; }
}
public class Wa2Resource
{
	public static string ResPath;
	// public static void Clear()
	// {
	// 	SoundDic.Clear();
	// 	ImageDic.Clear();

	// }
	// public static Dictionary<string, AudioStream> SoundDic { get; private set; } = new();
	// // public static Dictionary<string, FileAccess> PakDir { get; private set; } = new();
	public static Dictionary<string, FileEntry> FileDic { get; private set; } = new();

	// iOS 性能优化：小文件字节缓存 + .pak 文件流复用，避免高频读取时重复 open/解压
	private static readonly Dictionary<string, byte[]> _bufferCache = new();
	private static readonly Queue<string> _bufferCacheKeys = new();
	private const int BufferCacheMaxCount = 512; // LRU 上限：长流程（如整周目）加载上千小文件时防止字典无限增长
	private static readonly Dictionary<string, System.IO.FileStream> _pakStreams = new();
	private const long BufferCacheMaxSize = 2 * 1024 * 1024; // 仅缓存 <=2MB 的小文件（贴图/配置），跳过音频/视频防止内存膨胀

	// iOS 性能优化：运行时 ImageTexture 缓存（Godot 不会缓存 CreateFromImage 的结果，
	// 而 VN 每次切场景都重读背景/立绘/CG，必须自己缓存避免反复解码+上传 GPU）。
	// 上限 128 张；溢出时只移除字典引用、不 Dispose，避免释放正在显示的纹理。
	private static readonly Dictionary<string, ImageTexture> _imageCache = new();
	private static readonly Queue<string> _imageCacheKeys = new();
	private const int ImageCacheMaxCount = 128;

	// ResPath 全局化路径缓存（避免每次 LoadFileBuffer/GetVoiceStream 都调 Godot API）
	private static string _cachedResPath, _cachedResPathGlobalized;
	private static string GlobalizedResPath
	{
		get
		{
			if (_cachedResPath != ResPath)
			{
				_cachedResPath = ResPath;
				_cachedResPathGlobalized = ProjectSettings.GlobalizePath(ResPath);
			}
			return _cachedResPathGlobalized;
		}
	}

	// iOS 性能优化：运行时解码的大图（背景/CG/立绘，来自 .pak 的 TGA/BMP）在上传 GPU 前
	// Godot 4.5 运行时 Compress 直接按 GPU 格式压缩：iOS 用 ASTC（显存降约 6-8x）。
	// 压缩结果随 _imageCache 缓存，仅首次解码付出一次 CPU 成本。压缩失败时退回未压缩纹理，绝不影响显示功能。
	private static void GpuCompressIfLarge(Image image)
	{
		Image.Format fmt = image.GetFormat();
		if ((fmt == Image.Format.Rgba8 || fmt == Image.Format.Rgb8)
			&& image.GetWidth() >= 512 && image.GetHeight() >= 512)
		{
			try
			{
				image.Compress(Image.CompressMode.Astc);
			}
			catch
			{
				// 压缩失败（如尺寸不满足块对齐）则保持未压缩，显示不受影响
			}
		}
	}

	private static ImageTexture CacheImage(string path, Image image)
	{
		GpuCompressIfLarge(image);
		ImageTexture tex = TryCreateTexture(image);
		if (tex == null)
		{
			// 压缩后建纹理失败（极少见，驱动/格式不支持），退回未压缩再试一次
			try { image.Decompress(); } catch { }
			tex = TryCreateTexture(image);
		}
		if (tex == null)
		{
			// 极端情况：返回 null，调用方对缺失纹理降级显示，避免整场景加载崩溃/黑屏
			return null;
		}
		tex.ResourceName = path;
		string key = path + "|" + Wa2EngineMain.Engine.EffectMode;
		_imageCache[key] = tex;
		_imageCacheKeys.Enqueue(key);
		while (_imageCacheKeys.Count > ImageCacheMaxCount)
		{
			string oldest = _imageCacheKeys.Dequeue();
			_imageCache.Remove(oldest);
		}
		return tex;
	}

	private static ImageTexture TryCreateTexture(Image image)
	{
		try
		{
			return ImageTexture.CreateFromImage(image);
		}
		catch
		{
			return null;
		}
	}
	public static AudioStream LoadOggSound(string path)
	{
		byte[] buffer = LoadFileBuffer(path);

		if (buffer == null)
		{
			return null;
		}
		AudioStream oggStream = AudioStreamOggVorbis.LoadFromBuffer(buffer);
		return oggStream;
		// SoundDic[path] = oggStream;
	}
	// public static VideoStream GetMovie(string name){
	// 	VideoStream video=new();
	// 	video.File=ResPath+"movie/"+name+"0.mp4";
	// 	return video;
	// }
	public class BgImage
	{
		public Texture2D texture;
		public string Effect;
		public string Mask;
	}
	public static AudioStream LoadWavSound(string path)
	{
		// GD.Print(path);
		byte[] buffer = LoadFileBuffer(path);
		if (buffer == null)
		{
			return null;
		}
		AudioStream wavStream = AudioStreamWav.LoadFromBuffer(buffer);
		return wavStream;

	}
	public static Texture2D GetTvImage(int id)
	{
		return GetTgaImage(string.Format("tv{0:D6}.tga", id));
	}
	public static Texture2D GetMaskImage(int id)
	{
		return GetBmpImage(string.Format("f0{0:D3}.bmp", id));
	}
	public static AudioStream GetVoiceStream(int label, int id, int chr)
	{
		// GD.Print(string.Format("{0:D4}_{1:D4}_{2:D2}.ogg", label, id, chr));
		return GetOggStream(string.Format("{0:D4}_{1:D4}_{2:D2}.ogg", label, id, chr));
	}
	public static AudioStream GetSeStream(int id)
	{
		if (FileDic.ContainsKey(string.Format("se_{0:D4}.wav", id)))
		{
			return GetWavStream(string.Format("se_{0:D4}.wav", id));
		}
		else
		{
			return GetOggStream(string.Format("se_{0:D4}.ogg", id));
		}
	}
	public static AudioStream GetOggStream(string path)
	{
		path = path.ToLower();
		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadOggSound(path);
	}
	public static ImageTexture GetCgImage(int id, int no)
	{
		string path = string.Format("v{0:D5}{1:D1}.tga", id, no);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetCgImage(int id)
	{
		string path = string.Format("v{0:D6}.tga", id);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetBgImage(int id, int type, int no)
	{
		string path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", id, no, type);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetChrImage(int id, int type)
	{
		string path = string.Format("{0:S}{1:D6}.tga", Wa2Def.CharDict[id], type);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetTgaImage(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadTgaImage(path);
	}
	public static ImageTexture GetBmpImage(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadBmpImage(path);
	}
	public static ImageTexture LoadTgaImage(string path)
	{
		// ulong start = Time.GetTicksMsec();
		path = path.ToLower();
		string key = path + "|" + Wa2EngineMain.Engine.EffectMode;
		if (_imageCache.TryGetValue(key, out var cached) && cached != null)
		{
			return cached;
		}
		byte[] buffer = LoadFileBuffer(path);
		// GD.Print(path);
		if (buffer == null)
		{
			return null;
		}
		Image image = new();
		image.LoadTgaFromBuffer(buffer);
		if (Wa2EngineMain.Engine.EffectMode != "")
		{
			SetImageEffect(image, buffer[17]);
		}
		return CacheImage(path, image);
	}
	public static void SetImageEffect(Image image, int depth)
	{
		byte[] data = image.GetData();
		byte[] bytes = LoadFileBuffer(Wa2EngineMain.Engine.EffectMode);
		if (bytes.Length == 1280)
		{
			if (bytes != null)
			{
				for (int i = 0; i < data.Length; i += 4)
				{
					int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
					data[i] = bytes[256 + gray];
					data[i + 1] = bytes[512 + gray];
					data[i + 2] = bytes[768 + gray];
				}
				image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
			}
		}
		else if (bytes.Length == 768)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
				data[i] = bytes[0 + gray];
				data[i + 1] = bytes[256 + gray];
				data[i + 2] = bytes[512 + gray];
			}
			image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
		}
		else if (bytes.Length == 256)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
				data[i] = bytes[gray];
				data[i + 1] = bytes[gray];
				data[i + 2] = bytes[gray];
			}
			image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
		}

	}
	public static ImageTexture LoadBmpImage(string path)
	{
		path = path.ToLower();
		byte[] buffer = LoadFileBuffer(path);

		if (buffer == null)
		{
			return null;
		}
		Image image = new();
		image.LoadBmpFromBuffer(buffer);
		image.Convert(Image.Format.Rgb8);
		return CacheImage(path, image);
	}
	public static AudioStream GetWavStream(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadWavSound(path);
	}

	public static AudioStream GetBgmStream(int id, bool loop = false)
	{
		if (GetOggStream(string.Format("BGM_{0:D3}.OGG", id)) != null)
		{
			return GetOggStream(string.Format("BGM_{0:D3}.OGG", id));
		}
		else
		{
			if (!loop)
			{
				return GetOggStream(string.Format("BGM_{0:D3}_A.OGG", id));
			}
			else
			{
				return GetOggStream(string.Format("BGM_{0:D3}_B.OGG", id));
			}
		}
	}
	public static byte[] LoadFileBuffer(string path)
	{
		path = path.ToLower();
		if (_bufferCache.TryGetValue(path, out var cached))
		{
			return cached;
		}

		FileEntry entry = FileDic.GetValueOrDefault(path);
		if (entry == null)
		{
			return null;
		}

		string fullPath = System.IO.Path.Combine(GlobalizedResPath, entry.PkgPath);

		if (!_pakStreams.TryGetValue(fullPath, out var fs) || fs == null)
		{
			fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			_pakStreams[fullPath] = fs;
		}

		byte[] result = null;
		using (var reader = new System.IO.BinaryReader(fs, System.Text.Encoding.Default, true))
		{
			fs.Seek(entry.Offset, System.IO.SeekOrigin.Begin);

			if (entry.Crypted == 0)
			{
				result = reader.ReadBytes((int)entry.Size);
			}
			else
			{
				// 解压缩流程
				byte[] arr = new byte[0x1000];
				for (int i = 0; i < 0xFEE; i++)
				{
					arr[i] = 0x20;
				}

				uint arr_w = 0xFEE;
				uint insize = 0, outsize = 0;

				uint inlim = reader.ReadUInt32();
				uint outlim = reader.ReadUInt32();

				byte[] readBuffer = reader.ReadBytes((int)inlim);
				byte[] buffer = new byte[outlim];
				bool complete = false;

				while (true)
				{
					if (insize >= inlim)
					{
						result = buffer;
						complete = true;
						break;
					}

					byte flag = readBuffer[insize++];

					for (int j = 0; j < 8; j++)
					{
						if (insize >= inlim || outsize >= outlim)
						{
							result = buffer;
							complete = true;
							break;
						}

						byte byte1 = readBuffer[insize++];

						if ((flag & 1) == 0)
						{
							byte byte2 = readBuffer[insize++];
							uint arr_r = (uint)(byte1 | (byte2 & 0xF0) << 4);
							uint counter = (uint)(byte2 & 0xF) + 3;

							while (counter-- > 0)
							{
								byte b = arr[arr_r++ & 0xFFF];
								arr[arr_w++ & 0xFFF] = b;
								buffer[outsize++] = b;
							}
						}
						else
						{
							arr[arr_w++ & 0xFFF] = byte1;
							buffer[outsize++] = byte1;
						}

						flag >>= 1;
						if (complete) break;
					}
					if (complete) break;
				}
			}
		}

		// 仅缓存小文件（贴图/配置），跳过音频/视频等大块数据，防止内存膨胀
		if (result != null && entry.Size <= BufferCacheMaxSize)
		{
			if (!_bufferCache.ContainsKey(path))
			{
				// 新条目入队；超过上限时按 LRU 移除最旧条目（仅去引用，byte[] 交给 GC）
				_bufferCacheKeys.Enqueue(path);
				while (_bufferCacheKeys.Count > BufferCacheMaxCount)
				{
					_bufferCache.Remove(_bufferCacheKeys.Dequeue());
				}
			}
			_bufferCache[path] = result;
		}
		return result;
	}
	public static void LoadPak(string path)
	{
		string fullPath = System.IO.Path.Combine(GlobalizedResPath, path);
		// GD.Print(ProjectSettings.GlobalizePath(ResPath));
		// GD.Print(path);
		try
		{
			using (System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
			{
				uint magic = reader.ReadUInt32();

				if (magic == 0x5041434B) // 'PACK'
				{
					reader.ReadUInt64(); // skip 8 bytes
					uint nentry = reader.ReadUInt32();

					// 条目在 pak 里是连续布局（从 16 开始、每条 44 字节），原来循环里的
					// fs.Seek(16 + i * 44) 恒等于当前位置，是空操作；但它会让 BinaryReader
					// 每读一条就丢弃 4KB 读缓冲并重新定位，13 个 pak 累积起来是真实的启动开销。
					// 改为纯顺序读；Shift-JIS 编码器也一并提到循环外复用。
					Encoding sjis = Encoding.GetEncoding("shift_jis");
					for (int i = 0; i < nentry; i++)
					{
						uint crypted = reader.ReadUInt32();

						byte[] nameBuffer = reader.ReadBytes(24);
						string fileName = sjis.GetString(nameBuffer).ToLower().Replace("\0", "");

						reader.ReadUInt64(); // skip 8 bytes

						uint offset = reader.ReadUInt32();
						uint size = reader.ReadUInt32();

						FileEntry entry = new()
						{
							PkgPath = path,
							Offset = offset,
							Size = size,
							Crypted = crypted,
							FileName = fileName
						};

						FileDic[fileName] = entry;
					}
				}
				else if (magic == 0x0043414C) // 'LAC\x00'
				{
					uint nentry = reader.ReadUInt32();

					// 同 PACK 分支：条目连续布局（从 8 开始、每条 40 字节），Seek 是空操作且会破坏读缓冲
					Encoding sjis = Encoding.GetEncoding("shift_jis");
					for (int i = 0; i < nentry; i++)
					{
						byte[] nameBuffer = reader.ReadBytes(32);
						for (int j = 0; j < nameBuffer.Length; j++)
						{
							if (nameBuffer[j] != 0)
							{
								nameBuffer[j] = (byte)(~nameBuffer[j] & 0xFF);
							}
						}

						string fileName = sjis.GetString(nameBuffer).ToLower().Replace("\0", "");

						uint size = reader.ReadUInt32();
						uint offset = reader.ReadUInt32();

						FileEntry entry = new()
						{
							PkgPath = path,
							Offset = offset,
							Size = size,
							Crypted = 0,
							FileName = fileName
						};

						FileDic[fileName] = entry;
					}
				}
			}
		}
		catch (System.IO.FileNotFoundException ex)
		{
			Wa2EngineMain.Engine.OpenErrorMessage("资源读取失败,\n文件" + fullPath + "不存在:\n" + ex.Message);
		}
		catch (System.UnauthorizedAccessException ex)
		{
			Wa2EngineMain.Engine.OpenErrorMessage("访问权限获取失败:\n" + ex.Message);
		}
		catch (System.IO.IOException ex)
		{
			Wa2EngineMain.Engine.OpenErrorMessage("资源读取失败,\n文件" + fullPath + "已损坏:\n" + ex.Message);
		}

	}

	// 	var nentry=file.get_32()
	// 	for i in nentry:
	// 		file.seek(16+i*44*4)
	// 		var crypted=file.get_32()
	// 		var file_name=file.get_buffer(24*4).get_string_from_utf8()
	// 		file.get_64()
	// 		var offset=file.get_32()
	// 		var size=file.get_32()
	// 		print(file_name)
	// 		print(offset)
	// 		print(size)	
	// 	file.Close();
	// }
}
