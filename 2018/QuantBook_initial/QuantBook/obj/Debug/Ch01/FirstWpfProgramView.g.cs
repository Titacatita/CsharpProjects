﻿#pragma checksum "..\..\..\Ch01\FirstWpfProgramView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E551DACC4FB9B33B0B335091B7E90C073FB1EA1F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace QuantBook.Ch01 {
    
    
    /// <summary>
    /// FirstWpfProgramView
    /// </summary>
    public partial class FirstWpfProgramView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\Ch01\FirstWpfProgramView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txBlock;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\Ch01\FirstWpfProgramView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txBox;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Ch01\FirstWpfProgramView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnChangeColor;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Ch01\FirstWpfProgramView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnChangeSize;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/QuantBook;component/ch01/firstwpfprogramview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Ch01\FirstWpfProgramView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.txBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 13 "..\..\..\Ch01\FirstWpfProgramView.xaml"
            this.txBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnChangeColor = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\..\Ch01\FirstWpfProgramView.xaml"
            this.btnChangeColor.Click += new System.Windows.RoutedEventHandler(this.btnChangeColor_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnChangeSize = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\Ch01\FirstWpfProgramView.xaml"
            this.btnChangeSize.Click += new System.Windows.RoutedEventHandler(this.btnChangeSize_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
