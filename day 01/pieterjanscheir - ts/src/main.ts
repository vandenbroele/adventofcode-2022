import './style.css'
import input from "./input.txt?raw";

interface Elf extends Object {
  id: number;
  calories: number
}

let elves: Elf[] = [];
const inputArray: number[] = input.split(/\s{2}/).map((element) => parseInt(element, 10));

let amountOfCaloriesOfSelectedElf: number = 0;
let selectedElfId: number = 1;

for (let i = 0; i < inputArray.length; i++) {
  if (!isNaN(inputArray[i])) {
    amountOfCaloriesOfSelectedElf += inputArray[i];
  } else {
    let elf: Elf = {
      id: selectedElfId,
      calories: amountOfCaloriesOfSelectedElf,
    };
    elves.push(elf);
    selectedElfId++;
    amountOfCaloriesOfSelectedElf = 0;
  }
}
const maxAmountOfCalories: number = Math.max(...elves.map((o) => o.calories));
console.log(maxAmountOfCalories)

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
    <h1>${maxAmountOfCalories}</h1>
`
