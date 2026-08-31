using Godot;
using System.Threading.Tasks;


public partial class TitleMenu : Control
{
	// 点击跳过动画时不允许被打断的动画名（close 之后紧接着切场景，跳过会导致状态错乱）。
	private static readonly StringName SkipProtectedAnimation = "close";

	[Export]
	public Wa2Button CgModeButton;
	[Export]
	public Wa2Button BgmModeButton;
	[Export]
	public Control MenuBttons;
	[Export]
	public Control InitalStart;
	[Export]
	public AnimationPlayer AnimationPlayer;
	[Export]
	public Wa2Button StartButton;
	[Export]
	public Wa2Button LoadtButton;
	[Export]
	public Wa2Button OptionsButton;
	[Export]
	public Wa2Button QuitButton;
	[Export]
	public Wa2Button ICButton;
	[Export]
	public Wa2Button CcButton;
	[Export]
	public Wa2Button CodeaButton;
	[Export]
	public Wa2Button StartBackButton;
	[Export]
	public Wa2Button SpecialBackButton;
	[Export]
	public Wa2Button SpecialButton;
	[Export]
	public Control Special;
	[Export]
	public Control DigitalNovel;
	[Export]
	public Wa2Button DigitalNovelButton;
	[Export]
	public Wa2Button DigitalNovelBackButton;
	[Export]
	public Wa2Button DigitalNovel1Button;
	[Export]
	public Wa2Button DigitalNovel2Button;
	[Export]
	public Wa2Button SceneReplayButton;
	[Export]
	public Wa2Button VoiceMessageButton;
	private Wa2EngineMain _engine;


	public override void _Ready()
	{
		_engine = Wa2EngineMain.Engine;
		if (OS.GetName() == "iOS")
		{
			QuitButton.Disabled = true;
			QuitButton.Hide();
		}
		StartButton.ButtonDown += OnStartButtonDown;
		StartBackButton.ButtonDown += OnStartBackButtonDown;
		QuitButton.ButtonDown += OnQuitButtonDown;
		ICButton.ButtonDown += OnIcButtonDown;
		CcButton.ButtonDown += OnCCButtonDown;
		SpecialButton.ButtonDown += OnSpecialButtonDown;
		SpecialBackButton.ButtonDown += OnSpecialBackButtonDown;
		// As1Button.ButtonDown += OnAs1ButtonDown;
		// As2Button.ButtonDown += OnAs2ButtonDown;
		DigitalNovel1Button.ButtonDown += OnDigitalNovel1ButtonDown;
		DigitalNovel2Button.ButtonDown += OnDigitalNovel2ButtonDown;
		DigitalNovelButton.ButtonDown += OnDigitalNovelButtonDown;
		DigitalNovelBackButton.ButtonDown += OnDigitalNovelBackButtonDown;
		OptionsButton.ButtonDown += OnOptionsButtonDown;
		BgmModeButton.ButtonDown += OnBgmModeButtonDown;
		CodeaButton.ButtonDown += OnCodeaButtonDown;
		LoadtButton.ButtonDown += OnLoadButtonDown;
		CgModeButton.ButtonDown += OnCgModeButtonDown;
		SceneReplayButton.ButtonDown += OnSceneReplayButtonDown;
		VoiceMessageButton.ButtonDown+=OnVoiceMessageButtonDown;
	}
	public void SetResourcesReady(bool ready)
	{
		StartButton.Disabled = !ready;
		LoadtButton.Disabled = !ready;
		SpecialButton.Disabled = !ready;
	}
	private bool CanOpenContent()
	{
		return _engine != null && _engine.ResourcesReady;
	}
	public void OnSceneReplayButtonDown()
	{
		if (!CanOpenContent())
			return;
		_engine.UiMgr.OpenSceneReplayMenu();
	}
	public void OnVoiceMessageButtonDown()
	{
		if (!CanOpenContent())
			return;
		_engine.UiMgr.OpenVoiceMessageMenu();
	}
	public void OnDigitalNovelButtonDown()
	{
		if (!CanOpenContent())
			return;
		Special.Hide();
		DigitalNovel.Show();
	}
	public void OnDigitalNovelBackButtonDown()
	{
		DigitalNovel.Hide();
		Special.Show();

	}
	public void OnOptionsButtonDown()
	{
		_engine.UiMgr.OpenOptionsMenu();
	}
	public void OnCgModeButtonDown()
	{
		if (!CanOpenContent())
			return;
		_engine.UiMgr.OpenCGModeMenu();
	}
	public void OnBgmModeButtonDown()
	{
		if (!CanOpenContent())
			return;
		_engine.UiMgr.OpenBgmModeMenu();
	}
	public void OnLoadButtonDown()
	{
		if (!CanOpenContent())
			return;
		_engine.UiMgr.OpenLoadMenu();
	}
	public void OnSpecialButtonDown()
	{
		if (!CanOpenContent())
			return;
		MenuBttons.Hide();
		Special.Show();
	}
	public void OnCodeaButtonDown()
	{
		if (!CanOpenContent())
			return;
		Wa2EngineMain.RunGuarded(OnCodeaButtonDownAsync, "TitleMenu.OnCodeaButtonDown");
	}
	private async Task OnCodeaButtonDownAsync()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("3001");
		_engine.UiMgr.OpenGame();
	}
	public void OnCCButtonDown()
	{
		if (!CanOpenContent())
			return;
		Wa2EngineMain.RunGuarded(OnCCButtonDownAsync, "TitleMenu.OnCCButtonDown");
	}
	private async Task OnCCButtonDownAsync()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("2001");
		_engine.UiMgr.OpenGame();
	}
	public void OnDigitalNovel1ButtonDown()
	{
		if (!CanOpenContent())
			return;
		Wa2EngineMain.RunGuarded(OnDigitalNovel1ButtonDownAsync, "TitleMenu.OnDigitalNovel1ButtonDown");
	}
	private async Task OnDigitalNovel1ButtonDownAsync()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("5000");
		_engine.UiMgr.OpenGame();
	}
	public void OnDigitalNovel2ButtonDown()
	{
		if (!CanOpenContent())
			return;
		Wa2EngineMain.RunGuarded(OnDigitalNovel2ButtonDownAsync, "TitleMenu.OnDigitalNovel2ButtonDown");
	}
	private async Task OnDigitalNovel2ButtonDownAsync()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("5100");
		_engine.UiMgr.OpenGame();
	}
	public void OnIcButtonDown()
	{
		if (!CanOpenContent())
			return;
		Wa2EngineMain.RunGuarded(OnIcButtonDownAsync, "TitleMenu.OnIcButtonDown");
	}
	private async Task OnIcButtonDownAsync()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("1001");
		_engine.UiMgr.OpenGame();
	}
	public void OnQuitButtonDown()
	{
		GetTree().Quit();
	}
	public void OnStartBackButtonDown()
	{
		MenuBttons.Show();
		InitalStart.Hide();
	}
	public void OnSpecialBackButtonDown()
	{
		MenuBttons.Show();
		Special.Hide();
	}
	public void OnStartButtonDown()
	{
		if (!CanOpenContent())
			return;
		MenuBttons.Hide();
		InitalStart.Show();
	}
	public void Open()
	{
		SetResourcesReady(_engine.ResourcesReady);
		Show();
		Wa2EngineMain.RunGuarded(OpenAsync, "TitleMenu.Open");
	}
	private async Task OpenAsync()
	{
		AnimationPlayer.Play("RESET");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.SoundMgr.StopBgm();
		if (_engine.ReplayMode > 0 && _engine.ResourcesReady)
		{
			AnimationPlayer.Play("open");
			AnimationPlayer.Advance(AnimationPlayer.CurrentAnimationLength);
			_engine.UiMgr.OpenSceneReplayMenu();
		}
		else
		{
			// AnimationPlayer.Play("logo");
			// await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			AnimationPlayer.Play("open");
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			if (_engine.ResourcesReady)
			{
				_engine.SoundMgr.PlayBgm(31);
			}
		}
		
		_engine.ReplayMode = 0;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
	public override void _GuiInput(InputEvent @event)
	{

		if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			// 4.6 起 CurrentAnimation 由 string 变为 StringName，比较时用 StringName 常量避免依赖隐式转换。
			// 之前写法是 CurrentAnimation.Length —— 4.5 下取到的是「动画名的字符串长度」（"open"=4），
			// 并非动画时长；4.6 起 StringName 没有 Length 成员，会直接编译失败。
			if (AnimationPlayer.CurrentAnimation != SkipProtectedAnimation)
			{
				AnimationPlayer.Advance(AnimationPlayer.CurrentAnimationLength);
			}

		}

	}
}
