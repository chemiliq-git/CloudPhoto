var Student = /** @class */ (function () {
    function Student(firstName, middleInitial, lastName) {
        this.firstName = firstName;
        this.middleInitial = middleInitial;
        this.lastName = lastName;
        this.fullName = firstName + " " + middleInitial + " " + lastName;
    }
    return Student;
}());
var HelperTest = /** @class */ (function () {
    function HelperTest() {
    }
    HelperTest.prototype.greeter = function (person) {
        return "Hello, " + person.firstName + " " + person.lastName;
    };
    HelperTest.prototype.TSButton = function () {
        var name = "Fred";
        var user = new Student("Fred", "M.", "Smith");
        document.getElementById("ts-example").innerHTML = this.greeter(user);
    };
    return HelperTest;
}());
//# sourceMappingURL=app.js.map