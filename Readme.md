#Dlight-prototype
Dreitは、「初心者でも簡単に扱えるD言語ベースの言語」として構想していたプログラミング言語です。

初心者が簡単に学習でき、現代的なプログラミングの手法を学べ、実用するのにも十分な機能を備えた言語を目指して開発中です。

[プロジェクトのWikiページ](https://github.com/B-head/Dlight-prototype/wiki)にも情報がありますので、合わせてお読み下さい。

##開発目標
- プログラミング初心者でも比較的簡単に学習できること。教師・講師の確保のため、C系の文法を基にした文法であること。
- スクリプト言語のような簡潔な記述と、大規模開発のための厳密な記述の両方に対応すること。
- ライブラリ・フレームワークの使用の際に、言語との親和性が高く、冗長性のない記述を可能にする十分な機能を提供すること。
- 可能な限りパフォーマンスの良いプログラムが書けること。そのために必要な追加の記述が可能な限り少なくなること。

##主な採用パラダイム
- 手続き型
- 関数型
- 構造化プログラミング
- 契約プログラミング
- 純粋オブジェクト指向
- 属性指向
- ジェネリックプログラミング
- メタプログラミング

##コード例
###FizzBuzz
```
loop i <= 15 on var i := 1 by i += 1 {
    echo if i % 15 = 0 :: "fizzbuzz"
    else if i % 3 = 0 :: "fizz"
    else if i % 5 = 0 :: "buzz"
    else "{ i }"
}
```

###線形合同法乱数生成器
```
Random =: var rand //クラスインスタンスの生成と変数定義。
echo rand.value //フィールドを参照し出力。
echo rand.gen //メソッドを3回呼び出し出力。
echo rand.gen
echo rand.gen

class Random //クラス定義
{
	var value := 98765 //フィールド定義とデフォルト値の設定

	routine gen //メソッド定義
	{
		return value := value * 1103515245 + 12345
	}
}
```

このほかのコード例は、[DlightTest](https://github.com/B-head/Dlight-prototype/tree/master/DlightTest)のテストケースを御覧ください。
