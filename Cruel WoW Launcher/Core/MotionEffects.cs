using System.Windows;
using System.Windows.Media.Animation;

namespace Cruel_WoW_Launcher.Core
{
    class MotionEffects
    {
        private MainWindow WindowParent;

        public MotionEffects(MainWindow _window) => WindowParent = _window;

        public void FadeInControl(FrameworkElement control) => ((Storyboard)WindowParent.FindResource("FadeInControl")).Begin(control);

        public void FadeOutControl(FrameworkElement control) => ((Storyboard)WindowParent.FindResource("FadeOutControl")).Begin(control);
    }
}