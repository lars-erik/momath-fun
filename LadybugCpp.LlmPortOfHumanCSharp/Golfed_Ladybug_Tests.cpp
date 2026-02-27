#include "pch.h"
#include "CppUnitTest.h"
#include <string>
#include <vector>
import GolfedLadybug;

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace LadybugCppLlmPortOfHumanCSharp
{
    TEST_CLASS(LadybugCppLlmPortOfHumanCSharp)
    {
    public:

        TEST_METHOD(Procedural_ClockRun)
        {
            auto results = MakeRuns(1, 10);

            logResults(results);

            Assert::AreEqual(10, static_cast<int>(results.size()));
            int expected[] = { 4, 10, 8, 6, 6, 10, 5, 11, 4, 9 };
            for (int i = 0; i < 10; i++) Assert::AreEqual(expected[i], results[i]);
        }

        TEST_METHOD(Procedural_Frequency_Histogram)
        {
            auto results = MakeRuns(1, 10000);
            auto result = FromResults(results);

            for (const auto& [hour, frequency] : result.Frequencies)
                Assert::AreEqual(1.0 / 11.0, frequency, 0.0055);
        }

    private:
        static void logResults(const std::vector<int>& results)
        {
            std::wstring csv;
            for (int i = 0; i < (int)results.size(); i++)
            {
                if (i > 0) csv += L", ";
                csv += std::to_wstring(results[i]);
            }
            Logger::WriteMessage(csv.c_str());
        }
    };
}
