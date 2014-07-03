﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeeUI.Structs;
using GeeUI.Managers;

namespace GeeUI.Views
{
	/// <summary>
	/// An empty class used as a template. I guess.
	/// </summary>
	public class ListView : View
	{

		public NinePatch ContainerNinePatch = null;

		public int ScrollMultiplier = 10;

		private View HighestChild
		{
			get
			{
				View highest = null;
				foreach (var child in Children)
					if (highest == null || child.AbsoluteBoundBox.Top < highest.AbsoluteBoundBox.Top) highest = child;
				return highest;
			}
		}

		private View LowestChild
		{
			get
			{
				View lowest = null;
				foreach (var child in Children)
					if (lowest == null || child.AbsoluteBoundBox.Bottom > lowest.AbsoluteBoundBox.Bottom) lowest = child;
				return lowest;
			}
		}

		private View WidestChild
		{
			get
			{
				View widestChild = null;
				foreach (var child in Children)
				{
					if (widestChild == null) widestChild = child;
					if (child.AbsoluteBoundBox.Width > widestChild.AbsoluteBoundBox.Width) widestChild = child;
				}
				return widestChild;
			}
		}

		public Rectangle ChildrenBoundBox
		{
			get
			{
				if (Children.Count == 0) return new Rectangle(RealX, RealY, 0, 0);
				View firstChild = HighestChild;
				View lastChild = LowestChild;
				View widestChild = WidestChild;
				int width = widestChild.AbsoluteBoundBox.Width;
				int height = lastChild.AbsoluteBoundBox.Bottom - firstChild.AbsoluteBoundBox.Top;
				return new Rectangle(firstChild.AbsoluteX, firstChild.AbsoluteY, width, height);
			}
		}

		public ListView(GeeUIMain GeeUI, View rootView)
			: base(GeeUI, rootView)
		{
		}


		private void RecomputeOffset()
		{
			this.ContentOffset.Value = new Vector2(0, this.ContentOffset.Value.Y);
			if (ChildrenBoundBox.Height <= this.AbsoluteBoundBox.Height)
			{
				this.ContentOffset.Value = new Vector2(this.ContentOffset.Value.X, 0);
				return;
			}
			if (ChildrenBoundBox.Bottom < AbsoluteBoundBox.Bottom)
			{
				this.ContentOffset.Value += new Vector2(0, ChildrenBoundBox.Bottom - AbsoluteBoundBox.Bottom);
			}
			if (ChildrenBoundBox.Top > AbsoluteBoundBox.Top)
				this.ContentOffset.Value = new Vector2(this.ContentOffset.Value.X, 0);

		}

		public override void OnMScroll(Vector2 position, int scrollDelta, bool fromChild = false)
		{
			this.ContentOffset.Value -= new Vector2(0, scrollDelta * ScrollMultiplier);
			RecomputeOffset();
			base.OnMScroll(position, scrollDelta, fromChild);
		}

		public override void OnMClick(Vector2 position, bool fromChild = false)
		{
			base.OnMClick(position);
		}

		public override void OnMClickAway(bool fromChild = false)
		{
			base.OnMClickAway();
		}

		public override void OnMOver(bool fromChild = false)
		{
			base.OnMOver();
		}
		public override void OnMOff(bool fromChild = false)
		{
			base.OnMOff();
		}

		public override void Update(float dt)
		{
			RecomputeOffset();
			base.Update(dt);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ContainerNinePatch != null)
				ContainerNinePatch.Draw(spriteBatch, AbsolutePosition, Width, Height, 0f, EffectiveOpacity);
			base.Draw(spriteBatch);
		}
	}
}
