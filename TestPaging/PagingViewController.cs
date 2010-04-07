using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace MonoTouch.Dialog
{
	public class PagingViewController : UIViewController
	{
		public UITableViewStyle Style = UITableViewStyle.Grouped;
		UIView baseView;
		UIScrollView scrollView;
		UIPageControl pageControl;
		
		bool dirty;
		
		private static bool GetRotateEnabled()
		{
			if(NSUserDefaults.StandardUserDefaults.StringForKey("interfaceRotateEnabled") == null)
			{
				NSUserDefaults.StandardUserDefaults.SetBool(false,"interfaceRotateEnabled");				
			}
			
			return NSUserDefaults.StandardUserDefaults.BoolForKey("interfaceRotateEnabled");
		
		}
		
		 void CreatePanels()
		{
		    scrollView.Scrolled += ScrollViewScrolled;
		
		    int count = 10;
		    RectangleF scrollFrame = scrollView.Frame;
		    scrollFrame.Width = scrollFrame.Width * count;
		    scrollView.ContentSize = scrollFrame.Size;
		
		    for (int i=0; i<count; i++)
		    {
		        UILabel label = new UILabel();
		        label.TextColor = UIColor.Black;
		        label.TextAlignment = UITextAlignment.Center;
		        label.Text = i.ToString();
		
		        if (i % 2 == 0)
		            label.BackgroundColor = UIColor.Red;
		        else
		            label.BackgroundColor = UIColor.Blue;
		
		        RectangleF frame = scrollView.Frame;
		        PointF location = new PointF();
		        location.X = frame.Width * i;
		
		        frame.Location = location;
		        label.Frame = frame;
		
		        scrollView.AddSubview(label);
		    }
		
		    pageControl.Pages = count;
		}
		
		private void ScrollViewScrolled (object sender, EventArgs e)
		{
		    double page = Math.Floor((scrollView.ContentOffset.X - scrollView.Frame.Width / 2) / scrollView.Frame.Width) + 1;
		
		    pageControl.CurrentPage = (int)page;
		}
		
		private bool rotateUIEnabled = PagingViewController.GetRotateEnabled();
		
		public bool RotateUIEnabled
		{
			get{return rotateUIEnabled;}
			set{rotateUIEnabled = value;}
		}
		
		public override void LoadView ()
		{
			baseView = new UIView(UIScreen.MainScreen.Bounds);
			
			scrollView = new UIScrollView(new RectangleF(0,0,baseView.Bounds.Width,baseView.Bounds.Height-36)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin,
				AutosizesSubviews = true,
				PagingEnabled = true,
				ShowsHorizontalScrollIndicator=false,
				ShowsVerticalScrollIndicator=false
			};
			
			pageControl = new UIPageControl(new RectangleF(0,baseView.Bounds.Height-36,baseView.Bounds.Width,36));
			
			pageControl.ValueChanged += delegate(object sender, EventArgs e) {
				var pc = (UIPageControl)sender;
				double fromPage = Math.Floor((scrollView.ContentOffset.X - scrollView.Frame.Width / 2) / scrollView.Frame.Width) + 1;
				var toPage = pc.CurrentPage;
				var pageOffset = scrollView.Frame.Width*toPage;
				Console.WriteLine("fromPage " + fromPage + " toPage " + toPage);
				PointF p = new PointF(pageOffset, 0);
				scrollView.SetContentOffset(p,true);
			};
			
			CreatePanels();
			
			baseView.AddSubview(scrollView);
			baseView.AddSubview(pageControl);
			
			View = baseView;
			
		}
		
		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			
			scrollView.Frame = new RectangleF(0,0,baseView.Bounds.Width,baseView.Bounds.Height-36);
			pageControl.Frame = new RectangleF(0,baseView.Bounds.Height-36,baseView.Bounds.Width,36);
			
			int count = pageControl.Pages;
			
		    RectangleF scrollFrame = scrollView.Frame;
		    scrollFrame.Width = scrollFrame.Width * count;
		    scrollView.ContentSize = scrollFrame.Size;
		
		    for (int i=0; i<count; i++)
		    {
		        RectangleF frame = scrollView.Frame;
		        PointF location = new PointF();
		        location.X = frame.Width * i;
		
		        frame.Location = location;
		
				scrollView.Subviews[i].Frame = frame;
				
		    }
			
			float pageOffset = scrollView.Frame.Width*pageControl.CurrentPage;
				
			PointF p = new PointF(pageOffset, 0);
			scrollView.SetContentOffset(p,true);
			
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{	
			return rotateUIEnabled;
		
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
		}

		public event EventHandler ViewDissapearing;
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (ViewDissapearing != null)
				ViewDissapearing (this, EventArgs.Empty);
			
		}

		/// <summary>
		///     Creates a new DialogViewController from a RootElement and sets the push status
		/// </summary>
		public PagingViewController ():this(false){}
		
		public PagingViewController (bool AutoRotateUI)
		{
			this.rotateUIEnabled = AutoRotateUI;
		
		}
			
	}
	
}
