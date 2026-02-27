# Solutions to MoMath 2026 Monthly Mindbenders

The [National Museum of Mathematics](http://momath.org/mindbenders) are running a puzzle series
in collaboration with [3Blue1Brown](https://www.youtube.com/@3blue1brown).

I got [nerdsniped](https://www.linkedin.com/feed/update/urn:li:ugcPost:7431682001285410816?commentUrn=urn%3Ali%3Acomment%3A%28ugcPost%3A7431682001285410816%2C7432007976913301504%29&replyUrn=urn%3Ali%3Acomment%3A%28ugcPost%3A7431682001285410816%2C7432846443402407936%29&dashCommentUrn=urn%3Ali%3Afsd_comment%3A%287432007976913301504%2Curn%3Ali%3AugcPost%3A7431682001285410816%29&dashReplyUrn=urn%3Ali%3Afsd_comment%3A%287432846443402407936%2Curn%3Ali%3AugcPost%3A7431682001285410816%29)
on LinkedIn by a request to review a solution written by an LLM.  

Per feb. 26 I solved the Ladybug puzzle using TDD and OOP in C# to learn the problem.  
Then I golfed it in C# to see how "simple" it could become in order to port to C++.  
Finally I whipped and quarreled around with Claude Sonnet 4.6 to port it to C++.  

The LLM from the LinkedIn thread apparently made one _interesting_ solution in 3 hours.  
I did the same thing twice, plus ported it to C++ using an LLM - in the same time.  
I dare suggest that my solution(s) are also more compelling than the LLM version.  
Even the golfed one is more readable.

- [C# Tests for the first OOP attempt](./LadybugMonteCarlo.Tests/Monte_Carlo_Ladybug.cs)
- [C# OOP model for the first attempt](./LadybugMonteCarlo.Tests/OopLadybug.cs)
- [C# golf attempt](./LadybugMonteCarlo.Tests/Golfed_Ladybug.cs)
- [C++ golf port tests](./LadybugCpp.LlmPortOfHumanCSharp/Golfed_Ladybug_Tests.cpp)
- [C++ golf port API](./LadybugCpp.LlmPortOfHumanCSharp/GolfedLadybug.ixx)