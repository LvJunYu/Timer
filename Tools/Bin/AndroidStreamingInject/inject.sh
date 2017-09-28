#!/bin/bash
if [ $# != 2 ] ; then
    echo "Usage:inject.sh resPath apkPath"
    exit 1
fi

scriptPath=$(cd `dirname $0`; pwd)
echo $scriptPath
cd $scriptPath

keystorePath=../../../GameAClient/KeyChain/Android/JoyYouAndroidDis.keystore

resFullPath=$1
apkFullPath=$2
apkPath=`dirname $apkFullPath`
apkName=$(basename $apkFullPath)

echo apk $apkPath $apkName


echo $1 $2

cp -f $apkFullPath ./app.apk

java -jar -Duser.language=en "./apktool.jar" d app.apk -f

cp -f $resFullPath/* ./app/assets/

java -jar -Duser.language=en "./apktool.jar" b app -f

jarsigner -verbose -storepass JoyYou!! -keystore $keystorePath -signedjar ./signed_app.apk  ./app/dist/app.apk joyyoudis -digestalg SHA1 -sigalg MD5withRSA

mv -f ./signed_app.apk $apkPath/signed_$apkName

rm -fr ./app

echo Complete

