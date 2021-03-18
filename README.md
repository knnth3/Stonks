# Requirements:
Python 3.7.9  
pytorch 1.7.1  
com.mlagents 1.9.0  
mlagents 0.25.0

# Instalation Steps:
* Install python
* Install ml-agents from within unity package manager
*     pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
*     python -m pip install mlagents==0.25.0
# Usefull commands:
* venv\Scripts\activate
* mlagents-learn config/stonks_config.yaml --time-scale=1 --run-id=batch0_2 --initialize-from=batch0_1 --force

# ml-agent params:

    --resume => resumes training  
    --time-scale => The timescale used in the simulation  
    --run-id => The unique name used to identify a run-id  
    --initialize-from => If a previous brain was trained, then this tag can be used to continue training  
    --force => forces the api to override a currently existing run-id  
