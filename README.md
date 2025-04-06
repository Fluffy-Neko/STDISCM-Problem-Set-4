# [STDISCM] P4 - Distributed Fault Tolerance

### Specifications
Distributed systems allow for fault tolerance.  For this exercise you are tasked to create an online enrollment system with different services distributed across multiple nodes.

The system should have the following bare-minimum features:

1) Login/logout. Track sessions across different nodes. (Use OAuth / JWT)
2) View available courses.
3) Students to enroll to open courses.
4) Students to view previous grades.
5) Faculty to be able to upload grades.

The application should be web based using MVC.  The view will be a node on its own.  While the rest of the features / API / controllers will also be on a separate node.
Use of networked virtual machines/bare-metal computers is recommended.

When a node is down, on the features supported by that node should stop working, but the rest of the application should still work.

#### Bonus
> Implementation of redundant database / persistence layer

> Implementation of more features on its own node.

---
## Deliverables
- Source code
- Video Demonstration (test cases to be provided later)
- Source code + build/compilation steps
- Slides containing the following:

**Key implementation steps** <br>
Explanations on how fault tolerance is achieved



**TO RUN**
go to \OnlineEnrollmentSystem terminal
run on terminal : dotnet run
go to http://localhost:<insert specified number>/Home/Login