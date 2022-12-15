#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64
#define MAX_ROWS 4000000

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
    int i, j, k, start, end, distance, input_size, row1[LEN], col1[LEN], row2[LEN], col2[LEN], count, beacon, ranges_count;
    range **ranges;

    input_size = 0;
    while(scanf("%*s %*s %s %s %*s %*s %*s %*s %s %s", s[0], s[1], s[2], s[3]) != EOF){
        col1[input_size] = my_atoi(s[0]);
        row1[input_size] = my_atoi(s[1]);
        col2[input_size] = my_atoi(s[2]);
        row2[input_size] = my_atoi(s[3]);      
        input_size++;
    }

    for(j = 0; j <= MAX_ROWS; j++){
        printf("Row %d: ", j);

        ranges_count = 0;
        beacon = -1;
        for(k = 0; k < input_size; k++){
            distance = my_abs(col1[k] - col2[k]) + my_abs(row1[k] - row2[k]);
            if(contains(j, row1[k], row1[k] + distance) || contains(j, row1[k], row1[k] - distance)){
                start = col1[k] - distance + abs(row1[k] - j);
                end = col1[k] + distance - abs(row1[k] - j);
                
                if(ranges_count == 0) ranges = malloc(sizeof(range*));
                else ranges = realloc(ranges, (ranges_count + 1) * sizeof(range*));

                ranges[ranges_count++] = create(max(0, start), min(MAX_ROWS, end));
            }

            if(row2[k] == j) beacon = col2[k];
        }

        sort(ranges, ranges_count);

        count = 0;
        for(i = 0; i < ranges_count; i++){
            start = ranges[i]->start;
            end = ranges[i]->end;

            while(i + 1 < ranges_count && ranges[i + 1]->start <= end){
                end = max(end, ranges[++i]->end);
            }

            count += end - start + 1;
        }

        if(count != MAX_ROWS + 1) {
            printf("Beacon can be placed!\n");

            for(i = 0; i < ranges_count; i++){
                start = ranges[i]->start;
                end = ranges[i]->end;

                while(i + 1 < ranges_count && ranges[i + 1]->start <= end){
                    end = max(end, ranges[++i]->end);
                }

                printf("\t%d => %d\n", start, end);
            }

            break;
        } else {
            printf("Not possible!\n");
        }

        for(i = 0; i < ranges_count; i++) free(ranges[i]);
        free(ranges);
    }

    for(i = 0; i < ranges_count; i++) free(ranges[i]);
    free(ranges);
    
    return 0;
}