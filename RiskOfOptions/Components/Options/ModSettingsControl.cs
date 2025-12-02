using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2.UI;

namespace RiskOfOptions.Components.Options;

public abstract class ModSettingsControl<TValue> : ModSettingsControl<TValue, BaseOptionConfig>;

public abstract class ModSettingsControl<TValue, TOptionConfig> : ModSetting
    where TOptionConfig : BaseOptionConfig
{
    private MPEventSystemLocator _eventSystemLocator;
    private TValue _originalValue;
    private bool _valueChanged;

    private BaseOptionConfig.IsDisabledDelegate _isDisabled;
    private bool _disabled;
    private bool _restartRequired;
    
    protected TOptionConfig? Config { get; private set; }
        
    protected ITypedValueHolder<TValue> valueHolder;

    private UnityEngine.UI.RawImage modifiedIndicator;
    private readonly UnityEngine.Color nonDefaultColor = new UnityEngine.Color(122f/255, 176f/255, 255f/255);
    private readonly UnityEngine.Color hasChangedColor = new UnityEngine.Color(255f/255, 230f/255, 73f/255);

    public void SubmitValue(TValue newValue)
    {
        if (!_valueChanged)
            _originalValue = GetCurrentValue();
            
        _valueChanged = true;
            
        if (_originalValue.Equals(newValue))
            _valueChanged = false;
            
        valueHolder.Value = newValue;
            
        UpdateControls();
        optionController.OptionChanged();
    }

    protected TValue GetCurrentValue()
    {
        return valueHolder.Value;
    }

    public override bool HasChanged()
    {
        return _valueChanged;
    }

    public override void Revert()
    {
        if (!HasChanged())
        {
            UpdateControls();
            return;
        }

        SubmitValue(_originalValue);
        _valueChanged = false;
        UpdateControls();
    }

    public void ResetToDefault()
    {
        if (option is null)
            return;

        SubmitValue((TValue)option.ConfigEntry.DefaultValue);
    }

    protected override void Awake()
    {
        base.Awake();
            
        _eventSystemLocator = GetComponent<MPEventSystemLocator>();

        if (option == null)
            return;

        valueHolder ??= (ITypedValueHolder<TValue>)option;

        Config = (TOptionConfig)option.GetConfig();

        CreateModifiedIndicator();

        _restartRequired = Config.restartRequired;
            
        var isDisabled = Config.checkIfDisabled;
        if (isDisabled == null)
            return;

        _isDisabled = isDisabled;
    }

    protected override void Start()
    {
        base.Start();
            
        UpdateControls();
    }

    protected void OnEnable()
    {
        UpdateControls();
    }

    public override void CheckIfDisabled()
    {
        if (string.IsNullOrEmpty(settingToken))
            return;
            
        if (_isDisabled == null)
            return;

        var disabled = _isDisabled.Invoke();

        if (disabled && !_disabled)
        {
            Disable();
            _disabled = true;
        }
        else if (!disabled && _disabled)
        {
            Enable();
            _disabled = false;
        }
    }

    private void RestartRequiredCheck()
    {
        if (!_restartRequired)
            return;
            
        if (valueHolder.ValueChanged())
        {
            optionController.AddRestartRequired(settingToken);
        }
        else
        {
            optionController.RemoveRestartRequired(settingToken);
        }
    }

    protected bool InUpdateControls { get; private set; }

    protected void UpdateControls()
    {
        if (!this)
            return;

        if (string.IsNullOrEmpty(settingToken))
            return;

        if (InUpdateControls)
            return;

        CheckIfDisabled();
        RestartRequiredCheck();

        if (modifiedIndicator) {
            bool nonDefault = !GetCurrentValue().Equals((TValue)option.ConfigEntry.DefaultValue);
            modifiedIndicator.enabled = nonDefault || HasChanged();
            modifiedIndicator.color = HasChanged() ? hasChangedColor : nonDefaultColor;
        }

        InUpdateControls = true;
        OnUpdateControls();
        InUpdateControls = false;
    }
        
    protected virtual void OnUpdateControls() {}

    private void CreateModifiedIndicator()
    {
        UnityEngine.GameObject child = new UnityEngine.GameObject("Modified Indicator", typeof(UnityEngine.UI.RawImage));
        UnityEngine.RectTransform childTransform = (UnityEngine.RectTransform)child.transform;
        childTransform.SetParent(this.transform);
        childTransform.SetAsFirstSibling(); // to move to bottom layer, visually
        childTransform.pivot = new UnityEngine.Vector2(0, 0.5f);
        childTransform.anchoredPosition = UnityEngine.Vector2.zero;
        childTransform.anchorMax = new UnityEngine.Vector2(0.016f, 0.92f);
        childTransform.anchorMin = new UnityEngine.Vector2(0.009f, 0.08f);
        childTransform.sizeDelta = UnityEngine.Vector2.zero;

        modifiedIndicator = child.GetComponent<UnityEngine.UI.RawImage>();
        modifiedIndicator.color = nonDefaultColor;
    }
}