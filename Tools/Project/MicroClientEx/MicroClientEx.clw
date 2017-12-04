; CLW file contains information for the MFC ClassWizard

[General Info]
Version=1
LastClass=ContainerDialog
LastTemplate=CDialog
NewFileInclude1=#include "stdafx.h"
NewFileInclude2=#include "MicroClientEx.h"

ClassCount=5
Class1=CMicroClientExApp
Class2=CMicroClientExDlg

ResourceCount=4
Resource1=IDR_MAINFRAME
Class3=PngBtn
Class4=ProgressBar
Resource2=IDD_DIALOG_WEBBROWSER
Class5=ContainerDialog
Resource3=IDD_MICROCLIENTEX_DIALOG
Resource4=IDR_MENU1

[CLS:CMicroClientExApp]
Type=0
HeaderFile=MicroClientEx.h
ImplementationFile=MicroClientEx.cpp
Filter=N

[CLS:CMicroClientExDlg]
Type=0
HeaderFile=MicroClientExDlg.h
ImplementationFile=MicroClientExDlg.cpp
Filter=D
LastObject=CMicroClientExDlg
BaseClass=CDialog
VirtualFilter=dWC



[DLG:IDD_MICROCLIENTEX_DIALOG]
Type=1
Class=CMicroClientExDlg
ControlCount=3
Control1=IDOK,button,1342242817
Control2=IDCANCEL,button,1342242816
Control3=IDC_STATIC_INFO,static,1342308353

[CLS:PngBtn]
Type=0
HeaderFile=PngBtn.h
ImplementationFile=PngBtn.cpp
BaseClass=CDialog
Filter=D
LastObject=PngBtn

[CLS:ProgressBar]
Type=0
HeaderFile=ProgressBar.h
ImplementationFile=ProgressBar.cpp
BaseClass=CDialog
Filter=D
LastObject=ProgressBar

[DLG:IDD_DIALOG_WEBBROWSER]
Type=1
Class=ContainerDialog
ControlCount=2
Control1=IDC_EXPLORER,{8856F961-340A-11D0-A96B-00C04FD705A2},1342242816
Control2=IDC_HOTKEY1,msctls_hotkey32,1082195968

[CLS:ContainerDialog]
Type=0
HeaderFile=containerdialog.h
ImplementationFile=containerdialog.cpp
BaseClass=CDialog
LastObject=IDC_EXPLORER
Filter=D
VirtualFilter=dWC

[MNU:IDR_MENU1]
Type=1
Class=?
Command1=ID_MENUITEM_FULLSCR
Command2=ID_MENUITEM_BOSS
Command3=ID_MENUITEM_CLEAN
CommandCount=3

