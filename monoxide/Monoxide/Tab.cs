//
// Tab.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Gtk;

namespace Monoxide {

	public class Tab : HBox {
	
		private Widget content;
	
		public Tab (string tabName)
		{
			if (tabName == null)
				throw new ArgumentNullException ("tabName");

			Label label = new Label (tabName); 
			label.Justify = Justification.Left;
			label.UseUnderline = false;
			
			Gtk.Image image = new Gtk.Image (Stock.Close, IconSize.Menu);
			image.Xalign = 0.5f;
			image.Yalign = 0.5f;
 
			Button button = new Button ();
			button.HeightRequest = 20;
			button.WidthRequest = 20;
			button.Relief = ReliefStyle.None;
			button.CanFocus = false;
			button.Clicked += delegate {
				if (CloseButtonClicked != null)
					CloseButtonClicked (this, EventArgs.Empty);
			};
			button.Add (image);

			this.PackStart (label, true, true, 0);
			this.PackEnd (button, false, false, 0);
			this.ShowAll ();
		}
		
		public Widget Content {
			get { return content; }
			set { content = value; }
		}
		
		public event EventHandler CloseButtonClicked;
	}
}
