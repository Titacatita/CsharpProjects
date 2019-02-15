﻿#pragma checksum "..\..\..\Ch08\NNSimpleView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1CDE4F4C1817B1291DB1FE279F04D6B563D449A1"
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


namespace QuantBook.Ch08 {
    
    
    /// <summary>
    /// NNSimpleView
    /// </summary>
    public partial class NNSimpleView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox WindowSize;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PredictionSize;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton OriginalInput;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton NormalizedInput;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LoadData;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LearningRate;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Iterations;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Alpha;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox RelativeError;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton LMAlgorithm;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton RpropAlgorithm;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StartNN;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Ch08\NNSimpleView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Stop;
        
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
            System.Uri resourceLocater = new System.Uri("/QuantBook;component/ch08/nnsimpleview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Ch08\NNSimpleView.xaml"
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
            this.WindowSize = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.PredictionSize = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.OriginalInput = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 4:
            this.NormalizedInput = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 5:
            this.LoadData = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.LearningRate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.Iterations = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.Alpha = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.RelativeError = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.LMAlgorithm = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 11:
            this.RpropAlgorithm = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 12:
            this.StartNN = ((System.Windows.Controls.Button)(target));
            return;
            case 13:
            this.Stop = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
