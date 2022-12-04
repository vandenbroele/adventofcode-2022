let inputRaw = await fetch(`https://adventofcode.com/2022/day/4/input`).then(x => x.text());
let pairs = inputRaw.trim().split('\n').map(x => x.split(',').map(y => y.split('-')));
let toRange = x => { let out = []; for (let i = 0; i < x[1] - x[0] + 1; i++) out[i] = +x[0] + +i; return out; };
pairs = pairs.map(x => [toRange(x[0]), toRange(x[1])]);
let answer1 = pairs.filter(x => x[0].every(y => x[1].includes(y)) || x[1].every(y => x[0].includes(y))).length;
let answer2 = pairs.filter(x => x[0].some(y => x[1].includes(y))).length;

console.log(answer1, answer2);
