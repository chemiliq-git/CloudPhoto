

class Student {
    fullName: string;
    constructor(public firstName: string, public middleInitial: string, public lastName: string) {
        this.fullName = firstName + " " + middleInitial + " " + lastName;
    }
}

interface Person {
    firstName: string;
    lastName: string;
}

class HelperTest {

    greeter(person: Person) {
    return "Hello, " + person.firstName + " " + person.lastName;
}

TSButton() {
    let name: string = "Fred";
    let user = new Student("Fred", "M.", "Smith");

    document.getElementById("ts-example").innerHTML = this.greeter(user);
}
}