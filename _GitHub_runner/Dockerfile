FROM ubuntu

ARG DEBIAN_FRONTEND=noninteractive

RUN apt update -y && apt dist-upgrade -y \
    && apt install curl -y \
    && apt install -y --no-install-recommends \
    && apt install -y jq build-essential libssl-dev libffi-dev python3 python3-venv python3-dev python3-pip \
    && useradd -m docker

WORKDIR /home/docker
RUN mkdir actions-runner \
    && cd actions-runner \
    && curl -o actions-runner-linux-x64-2.311.0.tar.gz -L https://github.com/actions/runner/releases/download/v2.311.0/actions-runner-linux-x64-2.311.0.tar.gz \
    && tar xzf ./actions-runner-linux-x64-2.311.0.tar.gz \
    && chown -R docker ~docker \
    && /home/docker/actions-runner/bin/installdependencies.sh
USER docker
COPY entrypoint.sh .
ENTRYPOINT [ "./entrypoint.sh" ]