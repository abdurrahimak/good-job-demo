using System;

public interface ISectionState
{
    event Action SectionCompleted;
    event Action RestartLevel;

    void Begin();
    void End();
}
