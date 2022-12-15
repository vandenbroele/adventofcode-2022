#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64
#define ROW_TO_CHECK 2000000

#pragma region range
typedef struct r {
    int start, end;
} range;

range *create(int start, int end){
    range *r = malloc(sizeof(range));
    r->start = start;
    r->end = end; 
    return r;
}

void sort(range **ranges, int size){
    int i, j, min;
    range *r;

    for(i = 0; i < size - 1; i++){
        min = i;
        for(j = i + 1; j < size; j++){
            if(ranges[j]->start < ranges[min]->start || (ranges[j]->start == ranges[min]->start && ranges[j]->end < ranges[min]->end)){
                min = j;
            } 
        }

        r = ranges[i];
        ranges[i] = ranges[min];
        ranges[min] = r;
    }
}
#pragma endregion ranges

#pragma region helpers
int min(int a, int b){ return(a < b ? a : b); }

int max(int a, int b){ return(a < b ? b : a); }

int my_abs(int n) { return(n < 0 ? -n : n); }

int my_atoi(char *s){
    int i, value, inverse;

    for(i = 0, value = 0, inverse = 0; i < strlen(s); i++){
        if('0' <= s[i] && s[i] <= '9') value = (value * 10) + s[i] - '0';
        else if(s[i] == '-') inverse = 1;
    }

    if(inverse) value = -value;

    return value;
}

int contains(int n, int l, int r){
    if(r < l) return contains(n, r, l);
    else return(l <= n && n <= r);
}
#pragma endregion helpers

int main(){
    char s[4][LEN];
    int i, start, end, distance, row1, col1, row2, col2, count, beacon, ranges_count;
    range **ranges;
    
    while(scanf("%*s %*s %s %s %*s %*s %*s %*s %s %s", s[0], s[1], s[2], s[3]) != EOF){
        col1 = my_atoi(s[0]);
        row1 = my_atoi(s[1]);
        col2 = my_atoi(s[2]);
        row2 = my_atoi(s[3]);      

        distance = my_abs(col1 - col2) + my_abs(row1 - row2);
        if(contains(ROW_TO_CHECK, row1, row1 + distance) || contains(ROW_TO_CHECK, row1, row1 - distance)){
            start = col1 - distance + abs(row1 - ROW_TO_CHECK);
            end = col1 + distance - abs(row1 - ROW_TO_CHECK);
            
            if(ranges_count == 0) ranges = malloc(sizeof(range*));
            else ranges = realloc(ranges, (ranges_count + 1) * sizeof(range*));

            ranges[ranges_count++] = create(start, end);
        }

        if(row2 == ROW_TO_CHECK) beacon = col2;
    }

    sort(ranges, ranges_count);

    count = 0;
    for(i = 0; i < ranges_count; i++){
        start = ranges[i]->start;
        end = ranges[i]->end;

        while(i + 1 < ranges_count && ranges[i + 1]->start <= end){
            end = max(end, ranges[++i]->end);
        }

        count += end - start + 1 - contains(beacon, start, end);
    }

    printf("%d\n", count);

    for(i = 0; i < ranges_count; i++) free(ranges[i]);
    free(ranges);

    return 0;
}