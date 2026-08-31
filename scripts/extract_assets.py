#!/usr/bin/env python3
"""解压游戏素材 zip，正确还原中文/日文文件名（UTF-8）。

macOS 自带 unzip 对 zip 内的中文/日文文件名解码不良，会写出失败或产生乱码，
故改用 Python 解压：
  - 若 zip 条目已带 UTF-8 标记（general purpose bit 11），直接用文件名；
  - 否则按 cp437 重新解码为 UTF-8，还原原始文件名。
"""
import os
import sys
import zipfile


def main() -> int:
    if len(sys.argv) < 3:
        print("usage: extract_assets.py <zip> <outdir>", file=sys.stderr)
        return 2

    src, out = sys.argv[1], sys.argv[2]
    os.makedirs(out, exist_ok=True)

    with zipfile.ZipFile(src) as z:
        entries = z.infolist()
        for info in entries:
            if info.flag_bits & 0x800:
                name = info.filename                      # 已带 UTF-8 标记
            else:
                name = info.filename.encode("cp437").decode("utf-8", "replace")
            dest = os.path.join(out, name)
            if info.is_dir():
                os.makedirs(dest, exist_ok=True)
                continue
            parent = os.path.dirname(dest) or out
            os.makedirs(parent, exist_ok=True)
            with z.open(info) as s:
                data = s.read()
            # iOS 优化：注入的素材 .import 默认 vram_texture=false（原始 Android 工程设置），
            # 在 iOS 导出时翻成 true 以压成 ASTC，显存/体积更优。仅做字节级替换，不动其它内容。
            if dest.endswith(".import") and b'"vram_texture": false' in data:
                data = data.replace(b'"vram_texture": false', b'"vram_texture": true')
            with open(dest, "wb") as d:
                d.write(data)

    print("extracted %d entries -> %s" % (len(entries), out))
    return 0


if __name__ == "__main__":
    sys.exit(main())
