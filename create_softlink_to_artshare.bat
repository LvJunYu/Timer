@echo off
cd ../GameAClient/GameAClient/Assets/ArtCode
rd /S /Q ArtShareCode
mklink /D /J ArtShareCode "../../../../GameADesigner/GameMaker2D/Art/Assets/ArtCode/ArtShareCode"