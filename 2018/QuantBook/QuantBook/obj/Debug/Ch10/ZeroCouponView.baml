   M S B A M L   `   `   `  ����D  @QuantBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuantBook.Models.ChartModelQuantBook.Models.ChartModel  5      #    QuantBook.Ch10.ZeroCouponView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:QuantBook.Models.ChartModel     5      .��5	      � $��30q�6     � ColumnDefinitions  5
   
   �� 5      $	��200 �6   �� 5      .+�5   
   �� .+�5      �� -   5         ��Name$ StartZeroCoupon0��5      $��Start Zero Coupon 0��6/   $	��150��6M   $��0 0 0 0q�6Y   �� 5      -   5        $ StartZeroCoupon1��5      $��Start Zero Coupon 1��6/   $	��150��6M   $��0 10 0 0q�6Y   �� 5      -   5        $ StartZeroCoupon2��5      $��Start Zero Coupon 2��6/   $	��150��6M   $��0 10 0 0q�6Y   �� 5      -   5        $ StartInterBank��5      $��Start Interbank��6-   $	��150��6G   $��0 10 0 0q�6S   �� 5      $��Z-Spread (in bps):��6   $��0 30 0 0q�62   �� 5      -   5         ��Name$ ZSpread��5      $	��150��6'   $��0 5 0 0q�63   �� 5      -   5        $ StartZSpread��5      $��Start ZSpread��6+   $	��150��6C   $��0 10 0 0q�6O   5      � 5   
   $��1��6     5      �� 5      �� 5       � RowDefinitions 5      �� 5      �� 5      .+�5!      &   System.Windows.Controls.DataGrid     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6      CanUserAddRows .  $��0��6b      SelectionUnit$
 Cell=�6r   ��6A   �� *
ZcTable1+6r     5"      $
 Auto 6    .  $��1��6b   $
 Cell=�6r   ��6A   �� *
ZcTable2+6r   )   #QuantBook.Models.ChartModel.MsChart  5#         ChartBackground$
 Blue=�5$         Title$ Zero-Coupon Yield��63   $��0��6M   $��1��6]   	   XLabel$	 Times to Maturity��5%      
   YLabel$
 Zero-Coupon Rate��67      SeriesCollection 5#      �� *LineSeriesCollection1+   
DataSource 6O   �� *
ZcTable2+5%   7     5&      $	 Red=�6o   $ Discount Yield��5'      $��1��63   $��1��6C   $	 Times to Maturity��6P   $
 Discount Rate��6k    5&      �� *LineSeriesCollection2+ 6O   �� *
ZcTable2+5'   k   5(      5)      5*      