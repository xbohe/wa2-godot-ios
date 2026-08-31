using Godot;
using System;
[GlobalClass]
public partial class Wa2Button : TextureButton
{
	[Export]
	public AudioStream HoverStream;
	[Export]
	public AudioStream ClickStream;
	private Wa2EngineMain _engine;
	public override void _Ready()
	{
		_engine=Wa2EngineMain.Engine;
		ButtonDown+=OnClick;
		
	}
	public void OnClick(){
		_engine.SoundMgr.PlaySysSe(ClickStream);
	}
	private void OnHover(){
	}
	// 触摸模拟出的鼠标事件，其 device 恒为 -1（InputEvent.DEVICE_ID_EMULATION，
	// 官方文档 InputEvent.device 明确写了可据此区分模拟与物理输入）。
	private const int DeviceIdEmulation = -1;

	// 修「点击黏滞」：Godot 默认 emulate_mouse_from_touch=true，一次物理轻触会额外产生一份
	// 模拟的 InputEventMouseButton。关键点在于派发顺序 —— input.cpp 的 _parse_input_event_impl
	// 会在函数中段先派发这份模拟事件（device=-1），原始 InputEventScreenTouch 反而在函数末尾
	// 才派发。于是：模拟鼠标先到并激活「开始」，面板随即显示；紧接着 ScreenTouch 才到，
	// 这时它落在了刚刚显示出来的同位置控件上，于是 IC 被连带触发（特别模式→音乐模式同理）。
	// BaseButton 原生支持 InputEventScreenTouch（base_button.cpp 的 touch 分支），
	// 所以这里直接按 device 认出并吞掉那份模拟鼠标「按下」，真正的按下交给 ScreenTouch。
	// 只吞 MouseButton，不动 MouseMotion —— 因此选择肢的 MouseEntered 高亮与回溯界面的
	// 触摸滚动都不受影响；桌面物理鼠标的 device 是 32/0，也不受影响。
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed
			&& mb.Device == DeviceIdEmulation)
		{
			AcceptEvent();
			return;
		}
		base._GuiInput(@event);
	}
}
