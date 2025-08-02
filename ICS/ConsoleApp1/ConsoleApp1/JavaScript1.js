function called(firstname, lastname) {
    return `${firstname} ${lastname}`;
}


function callee() {
    const first = "Naveen";
    const last = "T";
    const fullname = called(first, last);
    console.log("Full Name:", fullname);
}


callee();
