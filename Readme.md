#Dlight-prototype

D言語のlight版を目指して構想していたプログラミング言語。

##コード例
```
Rand =: var rand
echo rand.value
echo rand.gen
echo rand.gen
echo rand.gen

class Rand
{
	var value := 98765

	routine gen
	{
		return value := value * 1103515245 + 12345
	}
}
```