﻿#pragma checksum "..\..\..\Ch03\GoogleFinanceView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1DBB994C2482D2A074745A475B6346D5"
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


namespace QuantBook.Ch03 {
    
    
    /// <summary>
    /// GoogleFinanceView
    /// </summary>
    public partial class GoogleFinanceView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Ticker;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button HistPrices;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RealtimeQuotes;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RecentOptions;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AllOptions;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid MyPrices;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid MyExpiries;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Ch03\GoogleFinanceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid MyOptions;
        
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
            System.Uri resourceLocater = new System.Uri("/QuantBook;component/ch03/googlefinanceview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Ch03\GoogleFinanceView.xaml"
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
            this.Ticker = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.HistPrices = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.RealtimeQuotes = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.RecentOptions = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.AllOptions = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.MyPrices = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.MyExpiries = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.MyOptions = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

