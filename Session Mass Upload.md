Session Mass Upload

Class CRN could not exist in database
Reasons could not exist in database
Student Information could not exist (email, id, firstName, lastName)
Technically Semester Code could not exist
Have to check for validity of each item

Course CRUD, People CRUD, Semester Code
if the Person hasn't signed in they have no Information in the system, need to enter it

if a course hasn't been used to sign in with, then that course does not exist,
need to enter it in manually

_Solutions_
Have a GetAllCourses from Banner Api to find out students courses, this mean each course that does not exist in the DB should get saved and each student should also get saved
When uploading, upload fails if banner fails. Probably will have worse error messages

Pro - This prevents people and course management
Con - Calls Banner Api, slows stuff down

Manual Course and People Management
When uploading, uploads fail if course or person does not exist. Must manually go and create records for each person and the associated course
When uploading, that persons schedule would be updated for that semester with that course

Flow -> A controller action that takes in a csv file, turn that csv file into a list of CSVUploadSessions. Then convert those objects into Session objects. Save objects to database. Any errors, cancel save

Controller
/sessions/upload (in: CSVFile) -> 200 OK OR Failure Messages

Check validity of all CSVSessionUploads
Convert CSVSessionUpload to Session
Add each Session to DB
Save
Report Errors or Return 200

Change Email to be a pkey on Person

CSVSessionUpload {
email: email,
inTime: DateTime,
outTime: DateTime,
tutoring: bool,
crns: List<int>,
reasons: List<String>,
semesterCode: int
}

CHECKLIST
backend

- [] Upload CSV Endpoint
- [] CRUD Course Endpoint
- [] CRUD Person Endpoint

frontend

- [] Upload CSV Page
- [] CRUD Course page
- [] CRUD Person page
