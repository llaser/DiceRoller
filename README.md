# DiceRoller

VRChatワールド用の、ダイス等をまとめて振るギミック。

## 各スクリプトについて

### DiceRoller

これをアタッチした`GameObject`をInteractする、または`RollDice()`を実行すると、設定したダイスの中で設定したコライダー内部にあるダイスを指定位置に移動します。

ダイスの向き・回転方向はランダムになります。出現位置は指定位置から指定距離以内のランダムな位置になります。

コライダー内にあるかどうかの判定は、ダイスのコライダー中心(ない場合は原点)と指定したコライダーの距離が0.001未満なら内部と判定されます。

- 設定内容
  - dice
    - 操作されうる`GameObject`を指定
  - referenceCollider
    - 振るダイスの判定に使用するコライダーを指定
    - 指定したコライダーは無効化され、Triggerになります
  - rollTarget
    - ダイスの移動先
  - rollRadius
    - ダイスが出現しうるrollTargetからの距離
    - rollTargetのローカルスケール
      - rollTargetのスケールを(1,0,1)のようにすると、出現範囲を円状に変えられます
  - rollAngularVelocityMagnitude
    - ダイス出現時の角速度の大きさ(グローバルスケール)
  - rollVelocity
    - ダイス出現時の速度(グローバルスケール)

### FlagDiscontinuityOnSleep

VRCObjectSyncの受信側でダイスが地面に引っかかり回転を反映できないことがあるので、送信側でSleepした次の同期時にFlagDiscontinuity()を実行するスクリプト。

VRCObjectSyncの挙動に依存しているため、将来的には機能しなくなる可能性あります。また、このスクリプトの必要がなくなる可能性もあります。

## Auther

llaser

## License

This work is licensed under the MIT license. See LICENSE for details.
Copyright (c) 2024 llaser
