   M S B A M L   `   `   `  ����  	QuantBook    QuantBook.MainView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ^ ZCaliburn.Micro.Platform, Version=2.0.2.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21f(calhttp://www.caliburnproject.org  5   	     � WindowState$  	Maximized=�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=�6!   .��5      ]� .+�5   
   �� $��25��6   $��Bottom=�6    $��0q�68   ��5      �� 5	      .R�5
      �  � RowDefinitions 5      �� 5      $��* �6,    � ColumnDefinitions 5      �� 5      $��45 �6/   �� 5      $	��110 �6/   �� 5      $��* �6/   �� 5      $	��200 �6/   5      5      .@�5      �� $��0��6   .��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��10,0,0,0q�6    $��Status:��62   $��10(�6A   
��� d �6O   5      5      �� 5      $��1��6   .��5      � -   5         �Name$ ProgressValue��5      $	��100��65   $��16��6A   $��0q�6M    � Minimum 6a   �� *ProgressMin+ � Maximum 5      �� *ProgressMax+5      �� 5       $��2��6   .��5!      �� -   5         ��Name$ 
StatusText��5!      $��10(�60   
��� d �6>   5"      �� 5$      $��3��6   $��Right=�6,   $��0,0,10,0q�6H   .��5%      �� $ 
Horizontal=�6   .+�5&      �� -   5        $ reportLabel��5&       $��10(�5'       
��� d �$��Center=�6E   ��5&   5   �� *SelectedItem.Header+	 �� ElementName$	 Items��5'   E   5(      5)      5*      � 5,   
   $��	10,0,10,0q�6    5-      �� 5.      $	��150 �6#   �� 5/      $��* �6#   .+�51      �� $
��Auto=�6   ��52      �� 53      
 �� 
StartPoint$	
 0,0&�6*    �� EndPoint$	 1,0&�6;   .��54      �  � Color$
 Gray��6'    � Offset$ 0Q�64   � 55      $ Transparent��6'   $ 1Q�6;   56      .��58      �� -   5         ��Name$ buttonPanel��58      $��0 10 0 0q�62   .+�59      �� $��	Chapter 1��6   $	��100��61   $��0 5 0 0q�6=   
������   Caliburn.Micro.Message   Attach$- '[Event Click]=[Action OnClick($source)]��6g   �� 5:      $��	Chapter 2��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5;      $��	Chapter 3��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5<      $��	Chapter 4��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5=      $��	Chapter 5��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5>      $��	Chapter 6��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5?      $��	Chapter 7��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5@      $��	Chapter 8��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5A      $��	Chapter 9��6   $	��100��61   $��0 5 0 0q�6=   
������ $- '[Event Click]=[Action OnClick($source)]��6g   �� 5B      $��
Chapter 10��6   $	��100��62   $��0 5 0 0q�6>   
������ $- '[Event Click]=[Action OnClick($source)]��6h   �� 5C      $��
Chapter 11��6   $	��100��62   $��0 5 0 0q�6>   
������ $- '[Event Click]=[Action OnClick($source)]��6h   5D      5E      �� 5F      -   5         ��Name$ Items��5F   )   $��1��6   6)   5G      5H      5I      