let inputRaw = await fetch(`https://adventofcode.com/2022/day/3/input`).then(x => x.text());
let sacks = inputRaw.trim().split('\n').map(x => x.split(''));

let getCompartmentOne = sack => sack.slice(0, sack.length/2);
let getCompartmentTwo = sack => sack.slice(sack.length/2);
let intersect = (arr1, arr2) => arr1.filter(x => arr2.includes(x));
let getCommonItem = sack => intersect(getCompartmentOne(sack), getCompartmentTwo(sack))[0];
let getPrio = x =>'_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'.indexOf(x);
let answer1 = sacks.map(getCommonItem).map(getPrio).reduce((sum, x) => sum + x);

let answer2 = 0;
for (let i = 0; i < sacks.length; i += 3) answer2 += getPrio([sacks[i], sacks[i + 1], sacks[i + 2]].reduce(intersect)[0]);

console.log(answer1, answer2);
