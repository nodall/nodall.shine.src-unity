using System;

using Nex.Core;

namespace Nex.Timing
{
    public class NClock : NObservable
    {
        #region [ ENUM ]
        public enum Status
        {
            STOP,
            PAUSE,
            PLAY,
        }
        #endregion

        #region [ Messages ]
        public static NPropertyMessageDescriptor<TimeSpan> RegionMinProperty = new NPropertyMessageDescriptor<TimeSpan>("RegionMin");
        public static NPropertyMessageDescriptor<TimeSpan> RegionMaxProperty = new NPropertyMessageDescriptor<TimeSpan>("RegionMax");
        public static NPropertyMessageDescriptor<TimeSpan> RegionStartProperty = new NPropertyMessageDescriptor<TimeSpan>("RegionStart");
        public static NPropertyMessageDescriptor<TimeSpan> RegionEndProperty = new NPropertyMessageDescriptor<TimeSpan>("RegionEnd");

        public static NPropertyMessageDescriptor<double> SpeedProperty = new NPropertyMessageDescriptor<double>("Speed");
        public static NPropertyMessageDescriptor<bool> IsLoopProperty = new NPropertyMessageDescriptor<bool>("IsLoop");
        public static NPropertyMessageDescriptor<bool> IsLimitProperty = new NPropertyMessageDescriptor<bool>("IsLimit");
        public static NPropertyMessageDescriptor<TimeSpan> LoopStartProperty = new NPropertyMessageDescriptor<TimeSpan>("LoopStart");
        public static NPropertyMessageDescriptor<TimeSpan> LoopEndProperty = new NPropertyMessageDescriptor<TimeSpan>("LoopEnd");
        public static NPropertyMessageDescriptor<NClock.Status> PlayerStateProperty = new NPropertyMessageDescriptor<NClock.Status>("PlayerState");

        public static NMessageDescriptor PlayMessage = new NMessageDescriptor();
        public static NMessageDescriptor StopMessage = new NMessageDescriptor();
        public static NMessageDescriptor PauseMessage = new NMessageDescriptor();
        public static NMessageDescriptor SeekMessage = new NMessageDescriptor( new NMessageArgument("Value", typeof(TimeSpan)));
        public static NMessageDescriptor TimeUpdatedMessage = new NMessageDescriptor();
        public static NMessageDescriptor LoopMessage = new NMessageDescriptor(
            new NMessageArgument("TimePlayerAtLastUpdate", typeof(TimeSpan)),
            new NMessageArgument("TimePlayer", typeof(TimeSpan)));
        public static NMessageDescriptor LimitMessage = new NMessageDescriptor(
            new NMessageArgument("TimePlayerAtLastUpdate", typeof(TimeSpan)),
            new NMessageArgument("TimePlayer", typeof(TimeSpan)));

        #endregion

        #region [ Fields ]
        // Time
        DateTime _time;                      // From localhost
        DateTime _timeAtLastUpdate;          // From localhost

        // TimeInterval
        TimeSpan _timeInterval;              // From StartTime to LocalHost
        TimeSpan _timeIntervalAtLastUpdate;  // From StartTime to LocalHost
        DateTime _startTime = new DateTime();

        // TimePlayer
        TimeSpan _timePlayer;                // From StartTime to LocalHost
        TimeSpan _timePlayerAtLasUpdate;     // From StartTime to LocalHost

        TimeSpan _regionMin = TimeSpan.Zero;
        TimeSpan _regionMax = TimeSpan.Zero;
        TimeSpan _regionStart = TimeSpan.Zero;
        TimeSpan _regionEnd = TimeSpan.Zero;

        TimeSpan _loopStart = TimeSpan.Zero;
        TimeSpan _loopEnd = TimeSpan.Zero;
        DateTime _timePause;
        DateTime _timePlay;

        NClock.Status _state = NClock.Status.STOP;
        NClock.Status _lastState = NClock.Status.STOP;
        double _speed = 1;
        bool _loop = false;
        bool _limits = false;
        TimeSpan _offset = new TimeSpan(0, 0, 0, 0, 10);

        #endregion

        #region [ Properties ]
        public DateTime StartTime { get { return _startTime; } }       
        public DateTime Now { get { return DateTime.Now; } }
        public DateTime Time { get { return _time; } }
        public DateTime TimeAtLastUpdate { get { return _timeAtLastUpdate; } }
        public TimeSpan TimeInterval { get { return _timeInterval; } }
        public TimeSpan TimeIntervalAtLastUpdate { get { return _timeIntervalAtLastUpdate; } }
        public TimeSpan TimePlayer { get { return _timePlayer; } }
        public TimeSpan TimePlayerAtLastUpdate { get { return _timePlayerAtLasUpdate; } }
        public DateTime DateTimePlayer { get { return _startTime + _timePlayer; } }
        public DateTime DateTimePlayerAtLastUpdate { get { return _startTime + _timePlayerAtLasUpdate; } }

        public TimeSpan Offset { get { return _offset; } set { _offset = value; } }

        public TimeSpan RegionMin
        {
            get { return _regionMin; }
            set
            {
                if (value > _regionStart)
                    RegionStart = value;

                TimeSpan oldValue = _regionMin;
                _regionMin = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, RegionMinProperty, oldValue, value));
            }
        }
        public TimeSpan RegionMax
        {
            get { return _regionMax; }
            set
            {
                if (value < _regionEnd)
                    RegionEnd = value;

                TimeSpan oldValue = _regionMax;
                _regionMax = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, RegionMaxProperty, oldValue, value));
            }
        }
        public TimeSpan RegionStart
        {
            get { return _regionStart; }
            set
            {
                if (value < _regionMin)
                    RegionMin = value;

                if (value > _regionEnd)
                    RegionEnd = value;

                TimeSpan oldValue = _regionStart;
                _regionStart = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, RegionStartProperty, oldValue, value));
            }
        }
        public TimeSpan RegionEnd
        {
            get { return _regionEnd; }
            set
            {
                if (value > _regionMax)
                    RegionMax = value;

                if (value < _regionStart)
                    RegionStart = value;

                TimeSpan oldValue = _regionEnd;
                _regionEnd = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, RegionEndProperty, oldValue, value));
            }
        }

        public double Speed
        {
            get { return _speed; }
            set
            {
                double oldValue = _speed;
                if (_state == NClock.Status.PLAY)
                {
                    Pause();
                    _speed = value;
                    Play();
                }
                else
                    _speed = value;


                NotifyMessage(new NPropertyMessage<double>(this, SpeedProperty, oldValue, value));
            }
        }
        public bool IsLoop { 
            get { return _loop; } 
            set 
            {
                bool oldValue = _loop;
                _loop = value;
                NotifyMessage(new NPropertyMessage<bool>(this, IsLoopProperty, oldValue, value)); 
            } 
        }
        public TimeSpan LoopStart
        {
            get { return _loopStart; }
            set
            {
                if (value > LoopEnd)
                    LoopEnd = value;

                TimeSpan oldValue = _loopStart;
                _loopStart = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, LoopStartProperty, oldValue, value)); 
            }
        }
        public TimeSpan LoopEnd
        {
            get { return _loopEnd; }
            set
            {
                if (value < _loopStart)
                    LoopStart = value;

                TimeSpan oldValue = _loopEnd;
                _loopEnd = value;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, LoopEndProperty, oldValue, value)); 
            }
        }
        public bool IsLimit 
        { 
            get { return _limits; } 
            set 
            {
                bool oldValue = _limits;
                _limits = value;
                NotifyMessage(new NPropertyMessage<bool>(this, IsLimitProperty, oldValue, value)); 
            } 
        }
        public NClock.Status PlayerState
        {
            get { return _state; }
            internal set
            {
                var oldValue = _state;
                _state = value;
                NotifyMessage(new NPropertyMessage<NClock.Status>(this, PlayerStateProperty, oldValue, value));
            }
        }
        #endregion

        #region [ Constructor ]
        public NClock()
        {
            Reset();
        }
        public NClock(object owner)
            :base(owner)
        {
            Reset();
        }
        #endregion

        #region [ Reset Method ]
        public void Reset()
        {
            DateTime now = DateTime.Now;

            _time = now;                                // From localhost
            _timeAtLastUpdate = now;                    // From localhost
            _timeInterval = TimeSpan.Zero;              // From StartTime to LocalHost
            _timeIntervalAtLastUpdate = TimeSpan.Zero;  // From StartTime to LocalHost
            _timePlayer = TimeSpan.Zero;                // From StartTime to LocalHost
            _timePlayerAtLasUpdate = TimeSpan.Zero;     // From StartTime to LocalHost

            _timePause = now;
            _timePlay = now;

            _regionMin = TimeSpan.Zero;
            _regionMax = TimeSpan.Zero;
            _regionStart = TimeSpan.Zero;
            _regionEnd = TimeSpan.Zero;

            _loopStart = TimeSpan.Zero;
            _loopEnd = TimeSpan.Zero;

            _startTime = now;

            _speed = 1;
            _loop = false;
            _limits = false;
        }
        #endregion

        #region [ Player ]
        public void Play()
        {
            if (_state != NClock.Status.PLAY)
            {
                _lastState = _state;
                PlayerState = NClock.Status.PLAY;
                _timePlay = _time;

                NotifyMessage(new NMessage(this, PlayMessage));
            }
        }
        public void Stop()
        {
            if (_state != NClock.Status.STOP)
            {
                _lastState = _state;
                PlayerState = NClock.Status.STOP;
                if (_limits)
                {
                    _timePause = (_speed >= 0) ? _startTime + LoopStart : _startTime + LoopEnd;
                }
                else
                {
                    _timePause = _startTime;
                }

                // Ponemos el tiempo en su ultimo Pause
                _timePlayer = (_timePause - _startTime);
                // Annulamos el interval de tiempo para que ningun evento se pueda disparar
                _timePlayerAtLasUpdate = _timePlayer;

                if ((_speed >= 0) && (IsLoop || IsLimit))
                {
                    _timePlayer = LoopStart;
                    _timePlayerAtLasUpdate = _timePlayer;
                }
                if ((_speed < 0) && (IsLoop || IsLimit))
                {
                    _timePlayer = LoopEnd;
                    _timePlayerAtLasUpdate = _timePlayer;
                }
                NotifyMessage(new NMessage(this, StopMessage));
            }
        }
        public void Pause()
        {
            if (_state != NClock.Status.PAUSE)
            {
                _lastState = _state;
                PlayerState = NClock.Status.PAUSE;
                _timePause = _startTime + _timePlayer;
                _timePlayer = _timePause - _startTime;
                NotifyMessage(new NMessage(this, PauseMessage));
            }
        }
        public void Seek(TimeSpan time)
        {
            _timePlay = _time;
            _timePause = _startTime + time;
            _timePlayer = _timePause - _startTime;

            NotifyMessage(new NMessage(this, SeekMessage, time));
            NotifyMessage(new NMessage(this, TimeUpdatedMessage));
        }
        public void UpdateStartTime()
        {
            UpdateStartTime(DateTime.Now);
        }
        public void UpdateStartTime(DateTime time)
        {
            _startTime = time;
            _timePause = _startTime;
            _timePlay = _startTime;
        }
        #endregion

        #region [ Update ]
        public void UpdateTime(DateTime now)
        {
            // UpdateTime
            _timeAtLastUpdate = _time;
            _time = now;

            // Update TimeInterval
            _timeIntervalAtLastUpdate = _timeInterval;
            _timeInterval = _time.Subtract(_startTime);

            // Update TimerPlayer
            _timePlayerAtLasUpdate = _timePlayer;
            switch (_state)
            {
                case NClock.Status.PLAY:
                    // Para solucionar el tema del primer frame que no se ejecuta
                    // este caso solo pasa cuando Pasamos de Play a Stop

                    _timePlayer = (_timePause - _startTime) + TimeSpan.FromTicks(Convert.ToInt64((double)(_time.Subtract(_timePlay).Ticks) * Speed));
                    if (IsLoop)
                    {
                        if ((_speed > 0) && (_timePlayer > _loopEnd))
                        {
                            _timePlay = _time;
                            _timePause = _startTime + _loopStart;

                            // Envia el tiempo antes de ser modificado por el loop por si existe algun KeyFrame a ejecutar
                            NotifyMessage(new NMessage(this, LoopMessage, _timePlayerAtLasUpdate, _timePlayer));

                            _timePlayerAtLasUpdate = _loopStart - _offset;
                            _timePlayer = _loopStart + _timePlayer.Subtract(_loopEnd);

                        }
                        else if ((_speed < 0) && (_timePlayer < _loopStart))
                        {
                            _timePlay = _time;
                            _timePause = _startTime + _loopEnd;

                            // Envia el tiempo antes de ser modificado por el loop por si existe algun KeyFrame a ejecutar
                            NotifyMessage(new NMessage(this, LoopMessage, _timePlayerAtLasUpdate, _timePlayer));

                            _timePlayerAtLasUpdate = _loopEnd + _offset;
                            _timePlayer = _loopEnd.Subtract(_loopStart.Subtract(_timePlayer));
                        }
                    }
                    else if (_limits)
                    {
                        if ((_speed > 0) && (_timePlayer > _loopEnd))
                        {
                            _timePause = _startTime + _loopEnd;
                            _timePlayer = _loopEnd;
                            PlayerState = NClock.Status.PAUSE;

                            NotifyMessage(new NMessage(this, LimitMessage, _timePlayerAtLasUpdate, _timePlayer));
                        }
                        else if ((Speed < 0) && (_timePlayer < _loopStart))
                        {
                            _timePause = _startTime + _loopStart;
                            _timePlayer = _loopStart;
                            PlayerState = NClock.Status.PAUSE;

                            NotifyMessage(new NMessage(this, LimitMessage, _timePlayerAtLasUpdate, _timePlayer));
                        }
                        if (_lastState == NClock.Status.STOP)
                        {
                            if (_speed > 0) _timePlayerAtLasUpdate = _timePlayerAtLasUpdate - _offset;
                            if (_speed < 0) _timePlayerAtLasUpdate = _timePlayerAtLasUpdate + _offset;
                        }
                        _lastState = NClock.Status.PLAY;
                    }
                    NotifyMessage(new NMessage(this, TimeUpdatedMessage));
                    break;
                case NClock.Status.STOP:                    
                    if (_lastState != NClock.Status.STOP)
                        NotifyMessage(new NMessage(this, TimeUpdatedMessage));

                    _lastState = NClock.Status.STOP;
                    break;
                case NClock.Status.PAUSE:                   
                    if (_lastState != NClock.Status.PAUSE)
                        NotifyMessage(new NMessage(this, TimeUpdatedMessage));

                    _lastState = NClock.Status.PAUSE;
                    break;
            }


        }
        public void UpdateTime()
        {
            UpdateTime(DateTime.Now);
        }
        #endregion

        #region [ IClonable ]
        public object Clone()
        {
            NClock newClock = new NClock();
            newClock._limits = _limits;
            newClock._loop = _loop;
            newClock._loopEnd = _loopEnd;
            newClock._loopStart = _loopStart;
            newClock._regionMin = _regionMin;
            newClock._regionMax = _regionMax;
            newClock._regionStart = _regionStart;
            newClock._regionEnd = _regionEnd;
            newClock._speed = _speed;
            newClock._startTime = _startTime;
            return newClock;
        }
        #endregion
    }
}
