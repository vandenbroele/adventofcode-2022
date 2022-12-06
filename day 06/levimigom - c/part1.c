#include <stdio.h>
#include <string.h>
#define LEN 5000

int main(){
	char s[5000];
	int i, duplicates, freq['z' - 'a' + 1] = { 0 };

	scanf("%s", s); 

	duplicates = 0;
	for(i = 0; i < strlen(s); i++){
		if(++freq[s[i] - 'a'] == 2) duplicates++;
		if(3 < i && --freq[s[i - 4] - 'a'] == 1) duplicates--;
		if(3 <= i && duplicates == 0) break;
	}

	printf("%d\n", i + 1);

	return 0;
}
