// sysinfo_overlay.m
// LiveContainer Tweak —— 通用系统信息悬浮窗（FPS / 本进程 CPU / 内存）
//
// 设计原则（参考 yyfll《不越狱给 iOS App 装 Tweak/插件》）：
//   * 纯 Objective-C + UIKit，不写 Logos，不链接 CydiaSubstrate，
//     避免 Theos 自动链接导致 TweakLoader 加载失败。
//   * 本插件只读取指标 + 画一个 UIWindow 悬浮层，不 hook 任何函数，
//     因此连 dlsym(MSHookFunction) 都不需要，最稳。
//   * __attribute__((constructor)) 为入口；LiveContainer 启动时由
//     TweakLoader 通过 dlopen 加载并执行构造函数。
//
// 适用：iOS 15+（LiveContainer 最低支持版本）至 iOS 26。

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <mach/mach.h>
#import <mach/thread_info.h>

#pragma mark - 指标采集

// 本进程 CPU 占用（%），对全部线程的 cpu_usage 求和，按 TH_USAGE_SCALE(1000) 归算。
static float app_cpu_percent(void) {
    thread_array_t thread_list = NULL;
    mach_msg_type_number_t thread_count = 0;
    kern_return_t kr = task_threads(mach_task_self(), &thread_list, &thread_count);
    if (kr != KERN_SUCCESS) return -1.0f;

    float total = 0.0f;
    for (mach_msg_type_number_t i = 0; i < thread_count; i++) {
        thread_basic_info_data_t info;
        mach_msg_type_number_t tcnt = THREAD_BASIC_INFO_COUNT;
        kr = thread_info(thread_list[i], THREAD_BASIC_INFO, (thread_info_t)&info, &tcnt);
        if (kr == KERN_SUCCESS && !(info.flags & TH_FLAGS_IDLE)) {
            total += info.cpu_usage;
        }
    }
    vm_deallocate(mach_task_self(), (vm_address_t)thread_list, thread_count * sizeof(thread_t));
    return total / 1000.0f; // TH_USAGE_SCALE == 1000
}

// 本进程物理内存占用（字节），task_vm_info.phys_footprint 最贴近“设置里看到的内存”。
static uint64_t app_mem_footprint_bytes(void) {
    task_vm_info_data_t vm;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    if (task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)&vm, &count) == KERN_SUCCESS) {
        return vm.phys_footprint;
    }
    return 0;
}

#pragma mark - 悬浮窗

@interface SysInfoOverlay : NSObject
@property (nonatomic, strong) UIWindow *window;
@property (nonatomic, strong) UILabel *label;
@property (nonatomic, strong) CADisplayLink *displayLink;
@property (nonatomic, assign) NSInteger frameCount;
@property (nonatomic, assign) CFAbsoluteTime lastFpsStamp;
@property (nonatomic, assign) float fps;
@end

@implementation SysInfoOverlay

- (void)start {
    CGFloat w = 156, h = 78;
    self.window = [[UIWindow alloc] initWithFrame:CGRectMake(8, 32, w, h)];
    if (@available(iOS 13.0, *)) {
        for (UIScene *s in UIApplication.sharedApplication.connectedScenes) {
            if ([s isKindOfClass:[UIWindowScene class]]) { self.window.windowScene = (UIWindowScene *)s; break; }
        }
    }
    self.window.windowLevel = UIWindowLevelAlert + 100;
    self.window.backgroundColor = [UIColor colorWithWhite:0.0 alpha:0.55];
    self.window.layer.cornerRadius = 8;
    self.window.clipsToBounds = YES;
    self.window.hidden = NO;

    self.label = [[UILabel alloc] initWithFrame:CGRectMake(6, 6, w - 12, h - 12)];
    self.label.numberOfLines = 0;
    self.label.font = [UIFont monospacedSystemFontOfSize:10 weight:UIFontWeightRegular];
    self.label.textColor = [UIColor colorWithRed:0.4 green:1.0 blue:0.5 alpha:1.0];
    self.label.userInteractionEnabled = YES;
    [self.window addSubview:self.label];

    UIPanGestureRecognizer *pan = [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(onPan:)];
    [self.label addGestureRecognizer:pan];

    self.frameCount = 0;
    self.lastFpsStamp = CFAbsoluteTimeGetCurrent();
    self.fps = 0;
    self.displayLink = [CADisplayLink displayLinkWithTarget:self selector:@selector(tick:)];
    [self.displayLink addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSRunLoopCommonModes];

    [NSTimer scheduledTimerWithTimeInterval:1.0 target:self selector:@selector(refresh) userInfo:nil repeats:YES];
}

- (void)onPan:(UIPanGestureRecognizer *)g {
    CGPoint p = [g locationInView:nil];
    static CGPoint off;
    if (g.state == UIGestureRecognizerStateBegan) {
        off = CGPointMake(p.x - self.window.frame.origin.x, p.y - self.window.frame.origin.y);
    } else if (g.state == UIGestureRecognizerStateChanged) {
        CGRect f = self.window.frame;
        f.origin.x = p.x - off.x;
        f.origin.y = p.y - off.y;
        self.window.frame = f;
    }
}

- (void)tick:(CADisplayLink *)link { (void)link; self.frameCount++; }

- (void)refresh {
    CFAbsoluteTime now = CFAbsoluteTimeGetCurrent();
    double dt = now - self.lastFpsStamp;
    if (dt > 0) self.fps = (float)(self.frameCount / dt);
    self.frameCount = 0;
    self.lastFpsStamp = now;

    double memMB = app_mem_footprint_bytes() / (1024.0 * 1024.0);
    float cpu = app_cpu_percent();
    self.label.text = [NSString stringWithFormat:
        @"FPS  %.0f\nCPU  %.1f%%\nMEM  %.1f MB", self.fps, cpu, memMB];
}

@end

#pragma mark - 入口

__attribute__((constructor))
static void sysinfo_overlay_entry(void) {
    // 延迟到主线程 RunLoop 就绪后创建 UI，避免构造函数早于 UIKit 初始化。
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(1.0 * NSEC_PER_SEC)),
                   dispatch_get_main_queue(), ^{
        static SysInfoOverlay *overlay = nil;
        if (!overlay) {
            overlay = [[SysInfoOverlay alloc] init];
            [overlay start];
            NSLog(@"[sysinfo-overlay] started");
        }
    });
}
