using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents m_GlobalHook;
        bool mouseUpEnabled = true;
        SoundPlayer player = new SoundPlayer("Gunshot.wav"); 

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        const int VK_DEL = 0x2E;
        void Delete()
        {
            keybd_event(VK_DEL, 0, 0, 0);
        }

        public MainWindow()
        {
            InitializeComponent();        
            player.Load();
            Subscribe();
            Hide();
        }

        public void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseMoveExt += EventMouseMove;
            m_GlobalHook.MouseUpExt += EventMouseUp;   
            
            System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
            icon.Icon = new System.Drawing.Icon("Icon.ico");

            System.Windows.Forms.ContextMenu cMenu = new System.Windows.Forms.ContextMenu();
            cMenu.MenuItems.Add("Snipe !", new EventHandler(Snipe));
            cMenu.MenuItems.Add("Exit", new EventHandler(Exit));

            icon.ContextMenu = cMenu;
            icon.Visible = true;
        }

        private void Snipe(object sender, EventArgs e)
        {
            Show();
            mouseUpEnabled = false;
        }

        private void Exit(object sender, EventArgs e)
        {
            Unsubscribe();
            Close();
        }

        private void EventMouseMove(object sender, MouseEventExtArgs e)
        {
            Left = e.X - 45;
            Top = e.Y - 45;
            Trace.WriteLine("X: " + e.X + " Y:" + e.Y);    
        }

        private void EventMouseUp(object sender, MouseEventExtArgs e)
        {
            DelayedPress();
            mouseUpEnabled = true;
        }


        public async void DelayedPress()
        {
            await Task.Delay(500);

            if (IsVisible && mouseUpEnabled)
            {
                player.Play();
                Delete();
            }
            
            if(mouseUpEnabled){
                Hide();        
                mouseUpEnabled = false;
            }
            
        }


        public void Unsubscribe()
        {
            m_GlobalHook.MouseMoveExt -= EventMouseMove;
            m_GlobalHook.MouseUpExt -= EventMouseUp;

            m_GlobalHook.Dispose();
        }
    }
}
