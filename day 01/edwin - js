let inputRaw = await fetch(`https://adventofcode.com/2022/day/1/input`).then(x => x.text());

let inputPerElf = inputRaw
    .split('\n\n')
    .map(x => x.split('\n').reduce((agg, y) => agg += +y, 0))
    .sort((a, b) => a - b);

console.log(
    inputPerElf.slice(-1)[0],
    inputPerElf.slice(-3).reduce((agg, y) => agg += +y, 0));
