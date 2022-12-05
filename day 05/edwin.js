let inputRaw = await fetch(`https://adventofcode.com/2022/day/5/input`).then(x => x.text());
let stacks = inputRaw.split('\n\n')[0].replaceAll('    ', ' [/]').split('\n').slice(0, -1).map(x => x.split(' ').map(y => y.charAt(1)));

function rotate(input) {
    let output = [], rows = input[0].length, cols = input.length;

    for (let i = 0; i < rows; i++) {
        output[i] = [];
        for (let j = 0; j < cols; j++) output[i][j] = stacks[cols - j - 1][i];
        output[i] = output[i].filter(x => x != '/');
    }
    
    return output;
}

let rotatedStacks = rotate(stacks);
let operations = inputRaw.split('\n\n')[1].trim().split('\n').map(x => ({ amnt: +x.split(' ')[1], src: +x.split(' ')[3] - 1, tgt: +x.split(' ')[5] - 1 }));

operations.forEach(x => {
    for (let i = 0; i < x.amnt; i++) {
        let popbox = rotatedStacks[x.src].pop();
        if (popbox) rotatedStacks[x.tgt].push(popbox);
    }
});

let rotatedStacks2 = rotate(stacks);

operations.forEach(x => {
    let src = rotatedStacks2[x.src];
    let cut = src.splice(src.length - x.amnt);
    rotatedStacks2[x.tgt].push(...cut);
});

let answer1 = rotatedStacks.map(x => x.slice(-1)).join('');
let answer2 = rotatedStacks2.map(x => x.slice(-1)).join('');

console.log(answer1, answer2);
