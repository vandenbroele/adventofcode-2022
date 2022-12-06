let inputRaw = await fetch(`https://adventofcode.com/2022/day/6/input`).then(x => x.text());
let signal = inputRaw.trim().split('');

function calc(len) {
    let answer = len;
    
    for (let i = 0; i < signal.length - len; i++, answer++) {
        let arr = signal.slice(i, i + len);
        if (arr.length == new Set(arr).size) break;
    }

    return answer;
}

console.log(calc(4), calc(14));
