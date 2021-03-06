﻿#pragma checksum "..\..\..\Ch11\SingleNameView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BF489FF7A56BF28DFB623F1A536D03D8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using QuantBook.Models.ChartModel;
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


namespace QuantBook.Ch11 {
    
    
    /// <summary>
    /// SingleNameView
    /// </summary>
    public partial class SingleNameView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Ticker;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox StartDate;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox EndDate;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MovingWindow;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PriceType;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox SignalType;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GetSignalData;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SignalIn;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SignalOut;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Notional;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox IsReinvest;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox StrategyType;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ComputePnL;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DrawdownStrategy;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DrawdownHold;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid SignalCollection;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PnLCollection;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\Ch11\SingleNameView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid YearlyPnLTable;
        
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
            System.Uri resourceLocater = new System.Uri("/QuantBook;component/ch11/singlenameview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Ch11\SingleNameView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.Ticker = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.StartDate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.EndDate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.MovingWindow = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.PriceType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.SignalType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.GetSignalData = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.SignalIn = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.SignalOut = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.Notional = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.IsReinvest = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            this.StrategyType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 13:
            this.ComputePnL = ((System.Windows.Controls.Button)(target));
            return;
            case 14:
            this.DrawdownStrategy = ((System.Windows.Controls.Button)(target));
            return;
            case 15:
            this.DrawdownHold = ((System.Windows.Controls.Button)(target));
            return;
            case 16:
            this.SignalCollection = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 17:
            this.PnLCollection = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 18:
            this.YearlyPnLTable = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

