# Cellarium
## CLI file manager for yandex drive

Cellarium is a command-line tool that allows users to synchronize files between a local directory and Yandex Drive. The tool is designed to be run on a Linux-based operating system and provides a variety of commands that can be used to manage and synchronize files.

Cellarium is also designed for backups transferring first.

`-cc or --clear_count [ external_path=/ max_count=10 delete_permanently=false ]` : This command is used to clear files by max count. The `external_path` parameter specifies the external path on Yandex Drive where the files are located. The `max_count` parameter specifies the maximum number of files to keep, and the `delete_permanently` parameter is used to specify whether the files should be deleted permanently or moved to trash.

`-ce or --clear_expired [ external_path=/ delta=30 delete_permanently=false ]` : This command is used to delete old files. The `external_path` parameter specifies the external path on Yandex Drive where the files are located. The delta parameter specifies the number of days after which a file is considered old, and the `delete_permanently` parameter is used to specify whether the files should be deleted permanently or moved to trash.

`-h or --help [ command= ]` : This command is used to show help. The `command` parameter is optional and can be used to show help for a specific command.

`--check_token` : This command is used to check if the token exists.

`--no_console` : This command is used to disable console output.

`-rmt or --remove_token` : This command is used to remove the Yandex Drive token.

`-st or --set_token [ token=XXX ]` : This command is used to set the Yandex Drive token. The token parameter is used to specify the token value.

`-s or --sync [ internal_path=/dev/null external_path=/ tag=<empty> force_create_external_path=false clear=true overwrite=false ]` : This command is used to synchronize the specified directory with Yandex Drive. The `internal_path` parameter specifies the local path of the files to be synced, the `external_path` parameter specifies the external path on Yandex Drive where the files will be synced to, the `tag` parameter is used to specify a tag for the files, the `force_create_external_path` parameter is used to force the creation of the external path if it does not exist, the `clear` parameter is used to clear the internal path after syncing and the `overwrite` parameter is used to specify whether files should be overwritten if they already exist.

`--verbose` : This command is used to enable verbose output.

`-v or --version` : This command is used to get the version of the Cellarium.