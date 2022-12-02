#include <stdio.h>

int nmod(int x, int y) { return(x < 0 ? x + y : x % y); }

int main(){
 	char a, b;
	int score = 0;
	
	while(scanf(" %c %c", &a, &b) != EOF){
		if(b == 'Y') score += 4 + a - 'A';
		else if(b == 'Z') score += 7 + (a - '@') % 3; 
		else score += 1 + nmod(a - 'B', 3);
	}

	printf("%d\n", score);

	return 0;
}
