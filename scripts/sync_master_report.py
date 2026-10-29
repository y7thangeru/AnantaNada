import os

def sync_report():
    with open('update_tracker.md', 'r', encoding='utf-8') as f:
        tracker = f.read()
    with open('Feature-ToBe-Implemented.md', 'r', encoding='utf-8') as f:
        features = f.read()
    with open('Report-To-GeminiProject.md', 'r', encoding='utf-8') as f:
        report = f.read()

    # Sync update_tracker
    start_str = "## File: `update_tracker.md`\n\n```markdown\n"
    end_str = "\n```\n\n---\n\n## File: `assets/ui_sprites/README.md`"
    if start_str in report and end_str in report:
        pre = report.split(start_str)[0]
        post = report.split(end_str)[1]
        report = pre + start_str + tracker + end_str + post
        print("update_tracker synced in report!")

    # Sync Feature-ToBe-Implemented
    f_start = "## File: `Feature-ToBe-Implemented.md`\n\n```markdown\n"
    f_end = "\n```\n\n---\n\n## File: `update_tracker.md`"
    if f_start in report and f_end in report:
        pre = report.split(f_start)[0]
        post = report.split(f_end)[1]
        report = pre + f_start + features + f_end + post
        print("Feature-ToBe-Implemented synced in report!")

    with open('Report-To-GeminiProject.md', 'w', encoding='utf-8') as f:
        f.write(report)
    print("Master report sync complete.")

if __name__ == '__main__':
    sync_report()
