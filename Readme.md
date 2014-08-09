#Dlight-prototype
Dlightは、初心者でも簡単に扱えるD言語を目指して構想していたものの、
いつの間にかD言語とは大きく違う文法になっていたプログラミング言語です。

[プロジェクトのWikiページ](https://github.com/B-head/Dlight-prototype/wiki)にも情報がありますので、合わせてお読み下さい。

##現在の目標
- 主要な静的型付け言語の基本的な機能、関数・クラス・テンプレートなどを実装する。
- 主要な動的型付け言語と同量程度のコードで、同等の動作を記述できるようにする。
- 今後の開発のために、開発・保守のしやすいコード設計を確立する。

##コード例
###FizzBuzz
```
//FizzBuzz
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
