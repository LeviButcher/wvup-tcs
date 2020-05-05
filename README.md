[![CircleCI](https://circleci.com/gh/LeviButcher/wvup-tcs/tree/master.svg?style=svg)](https://circleci.com/gh/LeviButcher/wvup-tcs/tree/master)

# Tutoring Center System

WVUP Tutoring Center System

# Working with tcs

It's recommended to use vscode while working with TCS for the best experience. At least with the frontend.

## Tools

### frontend

- Flow: Provides static type checking to Javascript
- ESlint: Static code analysis for Javascript
- React Testing Library: All react tests are written using it
- Prettier: Code formatting of HTML, CSS, and Javascript

### backend

- XUnit: Testing library for C#

If you want to understand the folder structure, check out the [Structure Document](https://github.com/LeviButcher/wvup-tcs/tree/master/Structure.md)

# Contributing Code to TCS

In order to merge code into master that will be deployed to production, a contributor must ensure all tests are passing locally in for both the frontend and backend. You can run 'dotnet test' and 'npm test' in the frontend and backend folders respectively to check the tests. Be aware that you must have a Postgres database connection string set up to run all the backend tests. If your tests pass then you will be able to make a pull request into master. Your branch will be checked for the following which will cause the pull to fail if one of these checks don't pass:

- Flow Type checks: Invalid type is being passed in somewhere then this check will fail
- Backend Compile Warning: If the backend compiles with warnings then this check will fail
- Frontend Unit Test: If a frontend test fails then this will fail
- Backend Unit Test: Same as frontend test check.
- Backend Integration Test: This test runs the backend Integration tests, if one of them fails then this fails

These Checks are ran on CircleCi in our CI/CD pipeline.

# TCS Deployment

TCS is deployed automatically using CircleCi, DockerHub, and Watchtower.
Check out the [deployment diagrams](https://drive.google.com/open?id=1M6NagP_pEK8mlKN8X7igNgJ-7MKpfJnvizLifOc3Hf0)
