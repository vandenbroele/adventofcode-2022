let inputRaw = await fetch(`https://adventofcode.com/2022/day/2/input`).then(x => x.text());
let games = inputRaw.trim().split('\n');

let mapping = { A: 'Rck', B: 'Ppr', C: 'Srs', X: 'Rck', Y: 'Ppr', Z: 'Srs' };
let points = { Rck: 1, Ppr: 2, Srs: 3 };
let isWin = { RckPpr: true, PprSrs: true, SrsRck: true };
let calcGamePoints = (i, j) => points[j] + (i === j ? 3 : isWin[i + j] ? 6 : 0);

let answer1 = games
    .map(x => x.split(' ').map(y => mapping[y]))
    .map(x => calcGamePoints(...x))
    .reduce((agg, x) => agg + x);

let mapping2 = {
    X: { Rck: 'Srs', Ppr: 'Rck', Srs: 'Ppr' },
    Y: { Rck: 'Rck', Ppr: 'Ppr', Srs: 'Srs' },
    Z: { Rck: 'Ppr', Ppr: 'Srs', Srs: 'Rck' }
};

let answer2 = games
    .map(x => x.split(' '))
    .map(x => [mapping[x[0]], x[1]])
    .map(x => [x[0], mapping2[x[1]][x[0]]])
    .map(x => calcGamePoints(...x))
    .reduce((agg, x) => agg + x);

console.log(answer1, answer2);
