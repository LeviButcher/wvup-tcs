[![CircleCI](https://circleci.com/gh/LeviButcher/wvup-tcs/tree/master.svg?style=svg)](https://circleci.com/gh/LeviButcher/wvup-tcs/tree/master)

# Tutoring Center System

WVUP Tutoring Center System

# Project Structure & Tools

The TCS Systems is composed of two parts. A React frontend that lives in the ./frontend. Then a dotnet core backend that lives in ./backend.

The React frontend is responsible for the kiosk pages, dashboard pages, and accepting card swipes.

The dotnet core backend handles all database calls, calling the Banner Api, and exposes out Rest Api endpoints for the frontend or other applications to request TCS data as JSON.

## React Frontend

<!-- The React frontend was scaffolded using Create-React-App. Create-React-App gives a base react application set up that doesn't involve having to mess with babel or any other javascript bundler/transpiler.

The React frontend uses several different tools to improve the development experience while working with react. It is recommended to use VSCode to edit the react frontend. It is able to take advantages of all the tools that it uses.

The react frontend uses the following tools: -->

### Folder Structure

The following folders make up the react frontend within the src folder:

- components: These files are react components that offer some common ui/functionality to be used throughout the site. Such as the Paging component to be used to Page over data.

- hooks: These files are react hooks that offer some common functionality to be used throughout the site. Such as the useCardRead hook that will listen for a card swipe and return it's dat. These hooks are to be used inside react components. Please read these docs to learn more about [hooks](https://reactjs.org/docs/hooks-intro.html)

- images: These are the images that the react frontend uses

- pages: The files contained inside pages are the literal pages of the application. There are two areas within the application, the dashboard and the kiosk. The kiosk pages are what students and teachers will see when they go to sign in at the kiosk. The dashboard pages are what the TCS employees will use to run reports and do lookups. There is a folder that corresponds to each area within pages that houses that given areas pages. such as the kiosk folder.

- schemas: These files are not react components. These are objects create by [yup](https://github.com/jquense/yup), which is a library that can do validation of objects. These schemas are used inside certain forms to make sure what was input is valid. If the input doesn't match the schema, then the form cannot be submitted.

- test-utils: These files are common utility functions that are used during are test suites. For example there is one file that contains a function that makes fetch return certain data when it is called.

- types: These files contains the different types of our application. The TCS frontend uses flow to provide types to our javascript, exactly like what typescript would provide. These files contains the types that match to the corresponding data that is returned by the dotnet core backend. For instance their is a type a Semester that contains a code and a name, which is exactly what the backend returns.

- ui: These files are common ui react components made using styled-components. Generally they shouldn't contain custom logic or functionality but only be used for visual purposes, or as elements within a larger component.

- utils: These files are commons utilities that are used within the app. Such as a function to check that a string is a wvup email.

## Dotnet Core Backend

Tools used

### Folder Structure

The dotnet core backend follows the standard dotnet core practices with folders, but with some slight variations within them but should be straight forward.

- Controllers: These files are the standard dotnet core controllers that handle the REST Api.

- EF: Contains the DBContext file and another file that handles seeding up sample data.

- Exceptions: The folder only contains one file which is AppException. This file has a class of TCSException which is the common exception thrown by all TCS backend code.

- Helpers: Common classes that can be reused through TCS. If a class didn't have a good folder to be put in, it usually got put here.

Migrations: Standard entity framework migrations

Models: The classes that make up our database tables are kept within this folder. Also we have a DTOs folder within here. The classes kept within DTOs are used when returning data from the REST API. DTO stands for Data Transfer Object, basically view models.

Properties: Keeps the launchSettings.json file, standard with all dotnet core web api.

Repos: The classes within here are what handle reaching out to the database to make different queries.

SampleData: The files kept within here are sql scripts that are ran by are DB initializer.

Services: The classes usually involve external service such as the BannerApi.

UnitOfWorks: The classes contained here usually take in several different repository or other classes and will return some result using them. It's used for Lookup up a person schedule that involves several repository and talking to the Banner Api

# TCS Deployment
