#!/bin/sh
/home/docker/actions-runner/config.sh --url $REPO_URL --token $RUNNER_TOKEN --unattended
/home/docker/actions-runner/run.sh