# NonaClip Version 1.0.3
------------------------

### 【概要】
NonaClipはクリップボードを拡張し、９個までのコピー履歴をホットキーを使って貼り付ける事ができるようになります。

### 【動作環境】
Windows7において動作確認を行いました。  
その他の環境における動作確認はしていません。

### 【インストール方法】
NonaClip.zipを解凍し、いずれかの場所にフォルダごとコピーしてください。
書き込みが禁止されているシステムフォルダなどにコピーした場合、設定とバッファの保存が正しく動作しないことがあります。

### 【アンインストール方法】
フォルダごと削除してください。  
（レジストリは使用していません）。

### 【起動方法】  
NonaClip.exeを実行します。

### 【使い方】
* クリップボード履歴の保存  
クリップボードの内容が変更された時、自動的に履歴に追加します。
以前の履歴は順番に繰り下げられ、最も後ろにある履歴が破棄されます。ただし、ロックされている履歴は番号が変化しません。

* 初期設定で以下のホットキーが利用できます。  
《Ctrl+1, Ctrl+2, ... Ctrl+9》  
対応する番号にある履歴をクリップボードにコピーした後、アクティブなウィンドウにペーストします。  
《Ctrl+0》  
表示／非表示の切り替えを行います。非表示の状態でもホットキーは有効です。

* ロック／アンロック  
フォームに表示された履歴をクリックすると、その履歴がロックされます。ロックされた履歴にはアスタリスク（*）マークが付き、番号が固定されます。もう一度クリックするとロックが解除されます。

* 設定変更  
タスクトレイのアイコンまたはフォームの右クリックメニューより、《設定》を選択します。各種ホットキーの配置などを変更する事ができます。

* 終了方法  
タスクトレイのアイコンまたはフォームの右クリックメニューより、《終了》を選択します。終了した際、履歴と設定は保存されます。


### 【注意点】  
* NonaClipはコピー／貼り付けの際にクリップボードを経由しますので、操作を行うとクリップボードの内容が置き換えられます。
* コピー／貼り付けを行うために、[Shift+Insert]または[Ctrl+v]を送ります。これらのホットキーに対応しないアプリケーションでは動作しません。
* このソフトを用いて貼り付けを行った際、編集キー（Shift、Controlなど）の状態がリセットされる事があります。いったんキーを離してから再度押してください。

### 【著作権および免責事項】  
本ソフトウェアの著作権はCannaryo(菅野亮)が保有します。 本ソフトはフリーソフトであり、配布に関する制限は設けません。配布にあたっては、アーカイブの内容を変更しないようにお願いします。また許可のない改変は禁止します。
このソフトウェアを使用したことによって生じたすべての障害・損害・不具合等に関しては、私と私の関係者および私の所属するいかなる団体・組織とも、一切の責任を負いません。各自の責任においてご使用ください。

### 【連絡先】  
不具合の報告や要望は以下のメールアドレスに送ってください。
メールアドレス： <ml_kanno@csc.jp>

### 【使用した素材など】  
本ソフトウェアの作成にあたり、以下のサイトより素材を利用させていただきました。

フレーム素材：  
びたちー素材館
<http://www.vita-chi.net/sozai1.htm>

シルエットイラスト：  
lagmaterial
<http://www.lagmaterial.com/>


### 【変更履歴】

#### ver1.0.1
* ホットキーで入力言語が切り替わってしまう問題を修正しました。
* コピー時に発生する遅延を一部改善しました。

#### ver1.0.2
* ホットキーによるコピーを廃止し、クリップボードの監視により履歴を作成するようにしました。
* [Ctrl+v]による貼り付けを設定可能にしました。
* テキストバッファを効率的な実装に変更しました。
* 背景グラフィックを追加しました。

#### ver1.0.3
* クリップボード変更のイベント発生時、テキストが実際に変更されているかチェックするようにしました（オプション）。

