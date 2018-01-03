# netHPC &middot; [![netHPC license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/glzbcrt/nethpc/blob/master/LICENSE)

This repository contains the final project I developed to earn my bachelor degree. It is a system I created back in 2013 to distribute the computation of algorithms between several computers. It is a big compute platform.

I developed it in C# using SQL Server as the data store and its management interface was created using Microsoft Management Console.

## Content

The **paper+presentation+demo** folder contains the paper and the presentation I used to present the project to the approval committee.
There is also a video showing how the solution works. All the content in this folder is in portuguese.

## Architecture and Modules

The following picture show the netHPC architecture. An installation is made of one head node and one or more compute nodes. The head node is responsible for managing the computing nodes and these ones are the ones where the real processing occurs.

![netHPC architecture](/media/arch01.png?raw=true "netHPC architecture")

The **src** folder contains the source code of the modules. The solution is made of the following modules:

* **netHPC.ManagementConsole**: the management console developed as a snap-in for Microsoft Management Console.
* **netHPC.SDK**: the SDK a developer should use to create an algorithm to be used in netHPC.
* **netHPC.Service**: this module contains both the compute and the head node services.
* **netHPC.Shared**: contains all the code shared between all modules.
* **netHPC.Samples.PrimeNumbers**: this is a sample algorithm I created to calculate the prime numbers in a range using the netHPC SDK.


## Results

You can view here the results of the prime numbers sample algorithm running on netHPC.
The charts were created running the algorithm to calculate the prime numbers between 1 an 5,000,000.

The first chart shows how the duration of the processing decreases as we add more cores and the work keeps almost the same as we add more cores. This behavior is expected as the fact of adding more cores doesn't change the work that must be done.

![Alt text](/media/chart01.png?raw=true "Title")

The second chart shows how the total work was broke in more work itens as we added more cores. This behavior is also expected and is necessary to keep all cores available busy.

![Alt text](/media/chart02.png?raw=true "Title")

## Status

This project is not finished and I will probably never finish it. I am sure there are lots of improvements to be made.

I am sharing it here so anyone can use it as-is or use it as a base for another project.
I am also sharing to show my coding skills in C# and big compute.