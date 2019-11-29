HTTPGET
/semesters
-> List of all Semesters in DB
[{code, displayName}]

HTTPGET
/person/{email}
/person/{wvupId}
-> Returns a person object that will be different depending on person type
EX: /person/student@wvup.edu
-> {
wvupId: 98432115,
email: student@wvup.edu,
firstName: Stacy
lastName: Cliff
schedule: [{
crn: 5,
semesterCode: 201902,
shortName: CS101,
name: Intro to computing
}],
personType: "Student"
}
NOTE: This should save the person to the database, save all the courses to the database and save that students schedule to the database
IF the person already exists and has a schedule for this semester then only return back what in our DB, don't call banner api

HTTPGET
/person/{email}/admin
/person/{id}/admin
NOTE: Returns the same format as /person/{email} and has the same conditions except it returns back ALL Classes the person has ever had not just the current semester

HTTPGET
/session?page=1

/session/{id}

HTTPPOST
/session
Request Body example -> For Student
{
personWVUPId: 2874413,
selectedClasses: [crn: "31546"] -> array of crn,
selectedReasons: [reasonId: 1] -> array of reasonId,
inTime: datetime,
outTime: datetime,
tutoring: false
}

HTTPUT
/session/{id}
Request Body Example -> For Student
{
id: 5
personWVUPId: 2874413,
selectedClasses: [crn: "31546"] -> array of crn,
selectedReasons: [reasonId: 1] -> array of reasonId,
inTime: datetime,
outTime: datetime,
tutoring: true
}

KIOSK API
HTTPPOST
/session/signIn
Request Body -> For Student
{
personWVUPId: 21546,
selectedClasses: [crn: "313546"],
selectedReasons: [reasonId: 1],
tutoring: true
}

/session/signOut
Request Body -> For Student
{
personWVUPId: 20164
}
