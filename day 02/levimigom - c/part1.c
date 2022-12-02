#include <stdio.h>

int main(){
	char a, b;
	int score = 0;

	while(scanf(" %c %c", &a, &b) != EOF){
		score += b - 'W';
	
		a -= 'A';	
		b -= 'X';

		if(a == b) score += 3;	
		else if(a - b == 2 || b - a == 1) score += 6;		
	}

	printf("%d\n", score);

	return 0;
}
