   M S B A M L   `   `   `  ����K  GInterviewManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:InterviewManagerInterviewManager  5   	   $    InterviewManager.ProfileWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:InterviewManager     5   	   -   5          � Title$  Profile��5   	   $	��300��5	   	   $	��300��5
   	    � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������  � 	Resources 5         ButtonStack �� 
TargetType �� Property �� Value �� 	Resources	 �� 
TargetType
 �� CornerRadius �� Triggers P� Property P� Value ��Name��%D  &	        '��     �� 5   
   " ��6$   .�5      ��  �@��Orientation$ 
Horizontal=�6,   6A   �� 5       � ��$ Right=�64   6D   �� 5       � ��$ Bottom=�62   6C   �� 5       � ��$ 1��6)   65   �� 5       � ��$ 	0 0 10 10q�6'   6;    5      ���� 5      '��       " ��6   .�5      ��  � ��$ 50��6.   6;   �� 5       � ��$ 20��6/   6<   �� 5       � ��$ 5q�6/   6;   �� 5       �@��CornerRadius$ 7��6<   �� 5       � �� 5      �� 5       "	 ��6.   .R�5!   "   �� #
 z
 6)   ���#ccc$��0,1,1,0q�6q   ��5"   &   �� 5#   *   " ��60   .�5$   .   ��  � �� �	#FFCCE8F36K    5%   .   P� 5&   2    �@J�IsMouseOver . . �5'   6   ��  � ��
 ��·�6S   5(   3   P� 5)   2    � {� .  . �5*   6   ��  � �� �#EFEFEF6S   5+   3   5-   +   ._�5/   &   �� $ contentPresenter��67   #��z��6Q   #��z��6�   #��z��6�   #��z��6�   #��z��6  50   #   51      53      54      55      56      �� 57   
   " ��6   .�58      ��  �@��MaxLines$	 500��6)   67   �� 59       �@��TextWrapping$
 Wrap=�6-   6<   �� 5:       �@��AcceptsReturn . 6=   �� 5;       �@��
AcceptsTab . 6:   �� 5<       �@��	IsEnabled . 6D   5=      5>      .��5?      �  � RowDefinitions 5@   
   �� 5A      6   �� 5B      $��40 �6   6*   .+�5D   
   �� $��2��6    J� Opacity$ 1Q�6%    �� RadiusX$ 25��61    �� RadiusY$ 25��6>   ��5E      �� 5F       �� EndPoint$ 0.4,1&�6&    � MappingMode$ RelativeToBoundingBox=�67    �� 
StartPoint$ 0.5,0.3&�6[   .��5G      �  � Color$ 	#B2000000��6#    � Offset$ 1Q�65   � 5H      $ 	#B2D4D4D4��6#   5I      5K      �� 5L   
   -   5        $
��Text��5L      $��20q�6   6,   �� 5M   
   #��[  6   .+�5N      �� -   5        $��Cancel��5N       �� IsCancel . .��63   Cancel6;   �� 5O      -   5        $��Ok��5O       �� 	IsDefault . .��6A   Ok6E   5P      5Q      5R      