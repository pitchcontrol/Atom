﻿using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Atom.Behavior
{
    public class FrameworkElementDropBehavior : Behavior<FrameworkElement>
    {
        private FrameworkElementAdorner adorner;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.DragEnter += AssociatedObject_DragEnter;
            this.AssociatedObject.DragOver += AssociatedObject_DragOver;
            this.AssociatedObject.DragLeave += AssociatedObject_DragLeave;
            this.AssociatedObject.Drop += AssociatedObject_Drop;
        }

        void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            Type type = typeof(WebPageBaseViewModel);
            //if the data type can be dropped 
            if (e.Data.GetDataPresent(type))
            {
                //drop the data
                WebPageBaseViewModel target = this.AssociatedObject.DataContext as WebPageBaseViewModel;
                if (target != null && target.IsDropable)
                {
                    target.Drop((WebPageBaseViewModel)e.Data.GetData(type));
                }
            }
            if (this.adorner != null)
                this.adorner.Remove();

            e.Handled = true;
            return;
        }

        void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            if (this.adorner != null)
                this.adorner.Remove();
            e.Handled = true;
        }

        void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            Type type = typeof(WebPageBaseViewModel);
            //if item can be dropped
            if (e.Data.GetDataPresent(type))
            {
                WebPageBaseViewModel target = this.AssociatedObject.DataContext as WebPageBaseViewModel;
                if (target != null)
                {
                    //give mouse effect
                    this.SetDragDropEffects(e);
                }
                //draw the dots
                if (this.adorner != null)
                    this.adorner.Update();
            }
            e.Handled = true;
        }

        void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {

            if (this.AssociatedObject.DataContext != null)
            {
                WebPageBaseViewModel dropObject = this.AssociatedObject.DataContext as WebPageBaseViewModel;
                if (dropObject != null)
                {
                    if (this.adorner == null)
                        this.adorner = new FrameworkElementAdorner(sender as UIElement);
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Provides feedback on if the data can be dropped
        /// </summary>
        /// <param name="e"></param>
        private void SetDragDropEffects(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;  //default to None
            Type type = typeof(WebPageBaseViewModel);
            //if the data type can be dropped 
            if (e.Data.GetDataPresent(type))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

    }

}
