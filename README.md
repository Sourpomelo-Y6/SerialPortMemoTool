# SerialportMemotool

シリアルポートがいつ繋がれたか記録を残す簡易ツール

主にM5stackのバッテリー管理用

とりあえず、シリアルポートIDが違えばできそう

# 必要な機能

つないだ時に時間とIDを記録する
->VIDとPID（にMI）が取得できたが全部同じ…でもポートは違う。

xmlファイルの保存と読み込み

たぶんスケッチ側の対応がいるだろうがバッテリー容量の確認


# 判別できるか調査


CtoCケーブル

M5StackBasic:COM5
M5StackFire(2):認識できず（USBハブ経由AtoCケーブルでのみ認識COM4）
M5StackFire(1):COM4
M5StickPlus:COM6
M5StackCore2:COM10

M5Paper:COM9

以下参考
Arduino Zero N 11
Arduino Zero P 12

種類が違えば識別できそう

# 